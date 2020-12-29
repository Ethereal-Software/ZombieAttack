
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/*
Walk:
----
Anim leng: 4 sec
Z movement: 1.5
...1.5/4 = 0.375 m/s 

run:
----
Anim leng: 24 frames at 30fps so 0.8 sec
Z movement: 2.65
...=3.3125 m/s
*/

public enum EnemyAgentType { 
    SpotAndCharge,
    ChargeFromAnywhere,
}

public enum MovementType { 
    Walking, 
    Running,
}

public class ZombieMovement : MonoBehaviour
{
    public Transform Zombie;
    public Animator animator;
    public float walkVelocity = 0.375f;
    public float runVelocity = 3.3125f;
    public Rigidbody rigidbody;
    public NavMeshAgent navMeshAgent;
    public AudioSource zombieSounds;

    public EnemyAgentType enemyAgentType;
    public float spotDistance = 100f;//only implemented for ChargeFromAnywhere
    public float seekDistance = 100f;

    public MovementType movementType;

    Transform target;
    //[HideInInspector]
    public bool isAlive = true;
    // Start is called before the first frame update

    public bool hasSetup = false;

    bool hasSeenTarget = false;
    Vector3 lastSeenAt;
    [HideInInspector]
    public bool isConstrained = false;

    public System.Action onDied;


    void Start()
    {
        nextShiftTime = Time.time + 4f;
        //animator.stabilizeFeet = true;

        float timeTp = Random.Range(0f, 1f);
        animator.SetFloat("offsetTime", timeTp);
        animator.SetTrigger("idle");

        

        //navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        navPath = new NavMeshPath();

        lastSeenAt = Zombie.position;
        hasSeenTarget = false;
        pathTarget =  Zombie.position;
        Debug.Log("setting path target: " + pathTarget.ToString());
        //AnimationEvent evt = new AnimationEvent();
        //evt.time = 0f;
        //evt.functionName = "PrintEvent";

        //var clip = animator.GetCurrentAnimatorStateInfo(0);
        //clip.
        //clip.AddEvent(evt);


        //error???
        target = PlayerHealth.thisPlayerHealth.transform;
    }

    float nextShiftTime = 0f;


    public void PrintEvent(int i) {
        Debug.Log("Got loop...");
    }

    // Update is called once per frame

    float nextFrameTime = 0f;


    public void Constrain() {
        isConstrained = true;
        navMeshAgent.Stop();
        animator.SetTrigger("idle");
    }

    NavMeshPath navPath;// = new NavMeshPath();
    Vector3 pathTarget;
    void SetDestination(Vector3 targetPosition) {
        if (targetPosition == null) return;
        if (navPath == null) {
            navPath = new NavMeshPath();
        }
        pathTarget = Zombie.position;
        NavMesh.CalculatePath(Zombie.position, targetPosition, NavMesh.AllAreas, navPath);

        var vectTp = new Vector3(0, 1f, 0);
        if (navPath.corners.Length > 1) {
            pathTarget = navPath.corners[1];//assume 1st fro now
            Debug.DrawLine(Zombie.position + vectTp, pathTarget + vectTp, Color.red);

        }
        
        //Debug.Log("corners?: " + navPath.corners.Length);
        //navPath.
        //for (int i = 0; i < navPath.corners.Length - 1; i++)
        //    Debug.DrawLine(navPath.corners[i]+ vectTp, navPath.corners[i + 1]+ vectTp, Color.red);

    }

    void MoveTowards(Vector3 targetPos) {

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //lerp to nav target
        var dir = targetPos - Zombie.position;
        //dir.y = 0;//ignore y
        
            //--------(Move toward)
        var velPerFrame = 0f;
        if (movementType == MovementType.Walking)
        {
            velPerFrame = walkVelocity * Time.deltaTime;
        }
        else if (movementType == MovementType.Running)
        {
            velPerFrame = runVelocity * Time.deltaTime;

        }
        //velPerFrame = runVelocity * Time.deltaTime;


        if (dir.magnitude < velPerFrame)
        {
            animator.SetTrigger("idle");
            zombieSounds.Stop();
            return;//to small, ignore
        }

        if (stateInfo.IsName("idle"))
        {
            animator.SetTrigger("walk");
            zombieSounds.Play();
        }


        //move position
        var dirNorm = dir.normalized;

        var posTp = Zombie.position;
        posTp += dirNorm * velPerFrame;
        Zombie.position = posTp;



        var yAng = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        var zomRot = Zombie.rotation.eulerAngles;
        var zomTarget = zomRot * 1;
        zomTarget.y = yAng;

        Zombie.rotation = Quaternion.Euler(zomTarget);
    }


    float nextNavUpdate = 0f;
    void Update() {
        if (!isAlive) {
            return;
        }

        if (target == null) {
            target = PlayerHealth.thisPlayerHealth.transform;
            return;
        }





        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //lerp to nav target
        MoveTowards(pathTarget);

        //---------------

        if (stateInfo.IsName("dying"))
        {
            zombieSounds.Stop();
            if (isAlive) {
                isAlive = false;
                onDied?.Invoke();
            }

            
        }

        if (!stateInfo.IsName("walking") && !stateInfo.IsName("running") && !stateInfo.IsName("idle")) return;




        if (!isConstrained)
        {
            // is in line of sight
            var origin = Zombie.position + new Vector3(0, 1.5f, 0);//eye height
            Vector3 dirTp = target.position - Zombie.position;
            RaycastHit hit;
            bool canSeeTarget = false;
            if (Physics.Raycast(origin, dirTp, out hit, seekDistance))
            {
                //Debug.Log("saw collider: " + hit.collider.gameObject.name);
                if (enemyAgentType == EnemyAgentType.SpotAndCharge)
                {
                    if (hit.collider.gameObject.transform == target)//spot and charge
                    {
                        hasSeenTarget = true;
                        lastSeenAt = target.position;
                        canSeeTarget = true;

                    }
                }
            }

            var dist = dirTp.magnitude;

            if (enemyAgentType == EnemyAgentType.SpotAndCharge)
            {
                if (hasSeenTarget && lastSeenAt != null)
                {
                    //navMeshAgent.SetDestination(lastSeenAt);

                    if (canSeeTarget)
                    {
                        SetDestination(lastSeenAt);
                    }
                    else
                    {//can't see, guess
                        var positionGuess = lastSeenAt + Zombie.forward * 10f;
                        SetDestination(positionGuess);
                    }
                    

                    //if (stateInfo.IsName("idle"))
                    //{
                    //    animator.SetTrigger("walk");
                    //    zombieSounds.Play();
                    //}
                }
            } else if (enemyAgentType == EnemyAgentType.ChargeFromAnywhere) {


                if (dist < spotDistance) {
                    hasSeenTarget = true;
                }

                if (hasSeenTarget)
                {
                    lastSeenAt = target.position;
                    //goto target anyway
                    //navMeshAgent.SetDestination(target.position);
                    //SetDestination(target.position);


                    //if (stateInfo.IsName("idle"))
                    //{
                    //    animator.SetTrigger("walk");
                    //    zombieSounds.Play();
                    //}
                }

                if (Time.time > nextNavUpdate) {
                    SetDestination(lastSeenAt);

                    nextNavUpdate += 0.3f;
                }


            }


        }


        return;

        //var dir = target.position - Zombie.position;

        ////var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //if (stateInfo.IsName("walking"))
        //{
        //    var dirNorm = dir.normalized;

        //    var posTp = Zombie.position;
        //    posTp += dirNorm * velocity * Time.deltaTime;
        //    Zombie.position = posTp;
        //}
        //else if (stateInfo.IsName("running"))
        //{
        //    var dirNorm = dir.normalized;

        //    var posTp = Zombie.position;
        //    posTp += dirNorm * runVelocity * Time.deltaTime;
        //    Zombie.position = posTp;
        //}
        //else {
        //    return;
        //}
        ////follow target...
        
        //var yAng = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        //var zomRot = Zombie.rotation.eulerAngles;
        //var zomTarget = zomRot * 1;
        //zomTarget.y = yAng;

        ////var newRot = Vector3.MoveTowards(zomRot, zomTarget, 5);//degrees

        //Zombie.rotation = Quaternion.Euler(zomTarget);
    }

    void LateUpdate_prev()
    {

        //var posTp = rigidbody.position;
        //posTp.z += velocity * Time.deltaTime;
        //rigidbody.MovePosition(posTp);

        var animStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        var frameTime = animStateInfo.normalizedTime;

        //Debug.Log(frameTime);
        if (frameTime > nextFrameTime) { //has looped
            var posTp = Zombie.position;
            posTp.z += 1.5f;
            Zombie.position = posTp;

            nextFrameTime += 1;
        }

    }
}
