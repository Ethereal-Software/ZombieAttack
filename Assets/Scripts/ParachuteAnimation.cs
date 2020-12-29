using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParachuteAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform parachute;
    public GameObject parachuteObject;
    public float swayAmount = 0.5f;
    public Transform carryObject;
    public Transform groundCheck;

    public float terminalVelocity = 2f;
    float curVelocity = 0f;
    public LayerMask collisionMask;
    bool hasLanded = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    float collapseScale = 1f;
    float collapseRotation = 0f;

    void Update()
    {
        if (hasLanded)
        {

            parachuteObject.SetActive(false);
            //if(collapseScale > 0.01f)
            //{
            //    collapseScale -= 1f * Time.deltaTime;
            //    parachute.localScale = new Vector3(collapseScale, 1f, collapseScale);
            //}
            //if (collapseRotation < 90) {
            //    collapseRotation += 90f * Time.deltaTime;
            //    parachute.rotation = Quaternion.Euler(collapseRotation, 0, collapseRotation);
            //}


            return;
        }

        if (curVelocity < terminalVelocity) {
            curVelocity += Time.deltaTime * Time.deltaTime * 9.8f;
        }
        else{
            curVelocity = terminalVelocity;
        }

        var posTp = parachute.position;
        posTp.y -= curVelocity;

        parachute.rotation = Quaternion.Euler(Mathf.Cos(Time.time) * Mathf.Rad2Deg* swayAmount, 0, Mathf.Sin(Time.time) * Mathf.Rad2Deg* swayAmount);

        parachute.position = posTp;


        if (Physics.CheckSphere(carryObject.position, 0.5f, collisionMask)) {
            hasLanded = true;
        }
        carryObject.rotation = Quaternion.Euler(0,0,0);

    }
}
