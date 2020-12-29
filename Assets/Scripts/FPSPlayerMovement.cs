using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Mirror;

//public class FPSPlayerMovement : MonoBehaviour
public class FPSPlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.8f;

    public Transform groundCheck;//empty object for ground collide
    public float groundDistance = 0.4f;//???
    public LayerMask groundMask;

    public float jumpHeight = 10;

    public Vector3 velocity;
    bool isGrounded;

    void Start(){
        //if(!isLocalPlayer){//turn off peer
            // PlayerController pc = GetComponent<PlayerController>();
            // if(pc != null){
            //     pc.enabled = false;
            // }
        //}
    }
    void Update(){
        //if(!isLocalPlayer) return;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0){
            //please fix
            velocity.y = 0;//-2f;//wtf??
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        controller.Move(move * speed * Time.deltaTime);
        
        //Debug.Log("Jump!!!"+ Input.GetButtonDown("Jump"));

        if(Input.GetButtonDown("Jump") && isGrounded){
            Debug.Log("Jumping?");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jumping 2?: " + velocity.y);
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
