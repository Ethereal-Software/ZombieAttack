using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera camera;
    public GameObject ZombiesObject;
    public ZombieColliders[] zombieColliders;

    public Collider spreadCollider;
    public FPSMouseLook fpsMouseLook;//for setting mouse sensitivity
    public float mouseSensitivity = 100f;
    public float mouseAimSensitivity = 20f;

    public Gun gun;
    public List<GameObject> guns;
    public int playerLayer;

    public LayerMask shootableMask;

    public float normalFOV = 60;
    //public float aimFOV = 40;

    public Vector3 gunRestPos = new Vector3(0.1f, 0,0);

    int gunIndex = 0;

    //public bool useSpread;

    //public float spreadAngle;//degrees
    //public int rayCount;

    //public AudioSource gunShot;
    //public AudioSource gunReload;
    //public float recoilTime;
    //public float reloadTime;
    //public int roundsPerLoad;

    //int roundsShot = 0;


    public GameObject bulletHoleProto;


    //public MuzzleFlare muzzleFlare;

    //float nextShootableTime = 0f;
    void Start()
    {
        zombieColliders = ZombiesObject.GetComponentsInChildren<ZombieColliders>();

        SetGunRestPosition();
    }

    public void UpdateZombieColliders() {
        zombieColliders = ZombiesObject.GetComponentsInChildren<ZombieColliders>();
    }

    void HideAllGuns() {
        foreach (GameObject go in guns) {
            if(go != null)
                go.SetActive(false);
        }
    }

    public void SetGun(int index){//GameObject newGunObj) {//don't check index

        if (guns[index] == null) return;

        //verify
        HideAllGuns();
        guns[index].SetActive(true);
        Gun gunTp = guns[index].GetComponent<Gun>();
        if (gunTp != null)
        {
            gun = gunTp;
        }
        gunIndex = index;
        SetGunRestPosition();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && guns.Count>0) {
            SetGun(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && guns.Count > 1)
        {
            SetGun(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && guns.Count > 2)
        {
            SetGun(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && guns.Count > 3)
        {
            SetGun(3);
        }
    }
    //bool needsReload = false;
    // Update is called once per frame


    void RaytraceShot() {

        Vector3 origin = Camera.main.transform.position;
        Vector3 directionMain = Camera.main.transform.forward.normalized;

        if (gun.useSpread)
        {//use spread instead
            Ray[] rays = new Ray[gun.rayCount];
            //create some random rays...


            Vector3 xVariant = Camera.main.transform.right.normalized;
            Vector3 yVariant = Camera.main.transform.up.normalized;
            float radius = Mathf.Sin(gun.spreadAngle * Mathf.Deg2Rad);

            float damageTp = gun.shotDamage / gun.rayCount;


            //
            for (int a = 0; a < gun.rayCount; a++)
            {
                float rTp = Random.Range(0f, radius);
                float angTp = Random.Range(0f, Mathf.PI * 2);
                //polar to cart..
                float xTp = Mathf.Sin(angTp) * rTp;
                float yTp = Mathf.Cos(angTp) * rTp;

                Vector3 dirTp = directionMain + xVariant * xTp + yVariant * yTp;
                Ray newRay = new Ray(origin, dirTp);
                rays[a] = newRay;
            }
            foreach (Ray rayTp in rays)
            {
                RaycastHit hit;

                if (Physics.Raycast(rayTp, out hit, 100, shootableMask))
                {
                    if (hit.collider.gameObject.layer != this.gameObject.layer)
                    {
                        var bHole = Instantiate(bulletHoleProto);
                        bHole.transform.position = hit.point;
                        bHole.transform.parent = hit.collider.transform;
                    }

                    foreach (ZombieColliders zombieCol in zombieColliders)
                    {
                        zombieCol.checkCollision(hit.collider, damageTp);
                    }
                }
            }
        }
        else// use raycast
        {


            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = new Ray(origin, directionMain);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, shootableMask))
            {

                Debug.Log("Shot: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.layer != this.gameObject.layer)
                {

                    var bHole = Instantiate(bulletHoleProto);
                    bHole.transform.position = hit.point;
                    bHole.transform.parent = hit.collider.transform;
                }

                foreach (ZombieColliders zombieCol in zombieColliders)
                {
                    zombieCol.checkCollision(hit.collider, gun.shotDamage);
                }
            }

            //testing
            //if (gun.projectileProto != null)
            //{
            //    Vector3 origin = Camera.main.transform.position;
            //    Vector3 directionMain = Camera.main.transform.forward.normalized;
            //    gun.LaunchProjectile(origin, directionMain);
            //}


        }
    }

    void SetGunRestPosition() {
        camera.fieldOfView = normalFOV;
        var posTp = gun.transform.localPosition;
        posTp.x = gunRestPos.x;
        posTp.y = gunRestPos.y;
        gun.transform.localPosition = posTp;
        fpsMouseLook.mouseSensitivity = mouseSensitivity;
    }

    void SetGunAimPosition()
    {
        camera.fieldOfView = gun.scopeFOV;
        var posTp = gun.transform.localPosition;
        posTp.x = 0f;
        posTp.y = 0f;
        gun.transform.localPosition = posTp;
        fpsMouseLook.mouseSensitivity = mouseAimSensitivity * gun.scopeFOV / normalFOV;
    }

    void Update()
    {
        //Ray ray;
        //Physics.Raycast()

        //if (nextShootableTime > Time.time) return;
        if (Input.GetMouseButton(1))
        { //look down scope
            SetGunAimPosition();

        }
        else if (camera.fieldOfView != normalFOV) {
            SetGunRestPosition();
        }


        var delta = Input.mouseScrollDelta;
        if (delta.y != 0) {
            Debug.Log("got delta..." + delta);
            var indexDelta = (int)delta.y;
            int newIndex = Mathf.Max(0,Mathf.Min(gunIndex+indexDelta, guns.Count-1));

            //var gunObj = guns[gunIndex];
            //var newGun = gunObj.GetComponent<Gun>();///????
            if (newIndex != gunIndex)
            {
                //gunIndex = newIndex;
                SetGun(newIndex);
            }

        }


        if (Input.GetMouseButtonUp(0))
        {
            gun.TriggerRelease();
            return;
        }

        if (!gun.CanShoot()) return;


        //if (needsReload) {
        //    needsReload = false;
        //    gunReload.Play();
        //    nextShootableTime = Time.time + reloadTime;
        //    return;
        //}



        if (Input.GetMouseButtonDown(0))
        {
            gun.onShoot = () => RaytraceShot();
            gun.Shoot();

            
        }
        //if (gun.isAutomatic && gun.isFiring)
        //{
        //    RaytraceShot();
        //    Debug.Log("tracing automatic");
        //}



    }
}
