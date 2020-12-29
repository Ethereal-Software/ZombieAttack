using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public AudioSource gunShot;
    public AudioSource gunReload;
    public AudioSource gunShotBurst;
    public float recoilTime;//seconds
    public float reloadTime;
    public int roundsPerLoad;
    public float shotDamage;


    public bool useSpread;

    public float spreadAngle;//degrees
    public int rayCount;

    public bool isAutomatic = false;

    int roundsShot = 0;

    public GameObject projectileProto;
    public float projectileSpeed;
    public float scopeFOV = 40f;

    //public GameObject bulletHoleProto;

    
    public MuzzleFlare muzzleFlare;

    float nextShootableTime = 0f;
    bool needsReload = false;
    // Start is called before the first frame update

    public bool CanShoot() {
        return nextShootableTime <= Time.time;
    }
    [HideInInspector]
    public Action onShoot;
    [HideInInspector]
    public bool isFiring = false;

    public GameObject curProjectile;
    public int zombieLayer;

    public void LaunchProjectile(Vector3 origin, Vector3 direction) {
        curProjectile = Instantiate(projectileProto);
        curProjectile.transform.forward = direction;
        curProjectile.transform.position = origin + direction;
        
    }

    public void Shoot() {
        muzzleFlare.Flash();
        isFiring = true;
        if (isAutomatic)
        {
            gunShotBurst.Play();
            
        }
        else
        {
            gunShot.Play();
            onShoot?.Invoke();
            isFiring = false;
            nextShootableTime = Time.time + recoilTime;
            roundsShot++;

            if (roundsShot % roundsPerLoad == roundsPerLoad - 1)//has shot last round
            {
                needsReload = true;
            }



        }




    }
    public void TriggerRelease() { //for automatic gun
        isFiring = false;
        if(gunShotBurst != null) gunShotBurst.Stop();
        onShoot = null;
        //gunShot.Play();
        //...
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (curProjectile != null) {

        //    Vector3 p1 = curProjectile.transform.position - curProjectile.transform.forward.normalized;
        //    Vector3 p2 = p1 - curProjectile.transform.forward.normalized*0.001f;
        //    Vector3 dir = curProjectile.transform.forward;
        //    RaycastHit hit;

        //    bool gotCollider = false;

        //    if (Physics.CapsuleCast(p1, p2, 0.01f, dir, out hit, 1f))
        //    {
        //        if (hit.collider.gameObject.layer != this.gameObject.layer)
        //        {
        //            if (hit.collider.gameObject.layer == zombieLayer) {
        //                var zombieRoot = hit.collider.GetComponentInParent<ZombieMovement>();
        //                if (zombieRoot != null)
        //                {
        //                    zombieRoot.Constrain();
        //                    zombieRoot.transform.parent = curProjectile.transform;
        //                    Debug.Log("got zombie root");
        //                }
        //            }
        //            else{
        //                curProjectile = null;
        //                gotCollider = true;
        //            }

        //            Debug.Log("coll: "+ hit.collider.gameObject.name+", "+hit.point.ToString());
        //            //has collided


        //        }
        //    }
        //    if(!gotCollider)
        //    {
        //        var dirTp = curProjectile.transform.forward.normalized;
        //        //dirTp.y -= Time.deltaTime * Time.deltaTime * 9.8f;


        //        curProjectile.transform.position += dirTp * Time.deltaTime * projectileSpeed;
        //        curProjectile.transform.forward = dirTp;
        //    }
        //}



        if (!CanShoot()) return;

        //auto reload
        if (needsReload)
        {
            needsReload = false;
            if(isAutomatic) gunShotBurst.Stop();
            gunReload.Play();
            nextShootableTime = Time.time + reloadTime;
            return;
        }

        if (isFiring && isAutomatic)
        {
            //if (CanShoot()) {
            if (isAutomatic && !gunShotBurst.isPlaying)
                gunShotBurst.Play();
            muzzleFlare.Flash();
            onShoot?.Invoke();
            nextShootableTime = Time.time + recoilTime;
            roundsShot++;

            if (roundsShot % roundsPerLoad == roundsPerLoad - 1)//has shot last round
            {
                needsReload = true;
            }

            //}
        }

    }

    //void RaytraceShot(GameObject bulletHoleProto, List<ZombieColliders> zombieColliders)
    //{

    //    if (useSpread)
    //    {//use spread instead
    //        Ray[] rays = new Ray[rayCount];
    //        //create some random rays...
    //        Vector3 origin = Camera.main.transform.position;
    //        Vector3 directionMain = Camera.main.transform.forward.normalized;

    //        Vector3 xVariant = Camera.main.transform.right.normalized;
    //        Vector3 yVariant = Camera.main.transform.up.normalized;
    //        float radius = Mathf.Sin(spreadAngle * Mathf.Deg2Rad);

    //        float damageTp = shotDamage / rayCount;


    //        //
    //        for (int a = 0; a < rayCount; a++)
    //        {
    //            float rTp = Random.Range(0f, radius);
    //            float angTp = Random.Range(0f, Mathf.PI * 2);
    //            //polar to cart..
    //            float xTp = Mathf.Sin(angTp) * rTp;
    //            float yTp = Mathf.Cos(angTp) * rTp;

    //            Vector3 dirTp = directionMain + xVariant * xTp + yVariant * yTp;
    //            Ray newRay = new Ray(origin, dirTp);
    //            rays[a] = newRay;
    //        }
    //        foreach (Ray rayTp in rays)
    //        {
    //            RaycastHit hit;

    //            if (Physics.Raycast(rayTp, out hit, 100))
    //            {
    //                var bHole = Instantiate(bulletHoleProto);
    //                bHole.transform.position = hit.point;
    //                bHole.transform.parent = hit.collider.transform;

    //                foreach (ZombieColliders zombieCol in zombieColliders)
    //                {
    //                    zombieCol.checkCollision(hit.collider, damageTp);
    //                }
    //            }
    //        }
    //    }
    //    else// use raycast
    //    {


    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit, 100))
    //        {

    //            var bHole = Instantiate(bulletHoleProto);
    //            bHole.transform.position = hit.point;
    //            bHole.transform.parent = hit.collider.transform;


    //            foreach (ZombieColliders zombieCol in zombieColliders)
    //            {
    //                zombieCol.checkCollision(hit.collider, shotDamage);
    //            }
    //        }
    //    }
    //}

}
