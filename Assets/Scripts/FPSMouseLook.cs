using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Mirror;


public class FPSMouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform camera;
    public KeyCode exitKey;
    float xRotation = 0f;
    bool isLocked = true;
    public GameObject inGameMenu;

    void Start(){
        // //if(!isLocalPlayer){
        //     Camera cam = camera.GetComponent<Camera>();
        //     cam.enabled = false;
        //     return;
        // //}
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //isLocked = true;
        Lock();
    }

    public void Lock() {
        Cursor.lockState = CursorLockMode.Locked;
        inGameMenu.SetActive(false);
        isLocked = true;
        Cursor.visible = false;
    }

    public void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        inGameMenu.SetActive(true);
        isLocked = false;
        Cursor.visible = true;
    }

    void Update(){

        if (Input.GetKeyDown(exitKey))
        {
            Unlock();

        }
        if (!isLocked) return;
        //if(!isLocalPlayer) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);


    }
}
