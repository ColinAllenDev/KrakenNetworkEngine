using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
<<<<<<< HEAD
    public PlayerManager player;
    public float sensitivity = 100f;
    public float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    private void Start()
    {
        // Lock Cursor
        ToggleCursorMode();
        
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
    }

    private void Update()
    {
        // Cursor Locking
        if(Input.GetKeyDown(KeyCode.Escape)) {
            ToggleCursorMode();
        }

        if(Cursor.lockState == CursorLockMode.Locked) {
            Look();
        }

        // Debug Stuff
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        float _mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }

    private void ToggleCursorMode() {
        Cursor.visible = !Cursor.visible;

        if(Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }


=======
    public Transform playerHead;

    void LateUpdate()
    {
        // Lock Position
        transform.position = playerHead.position;
        transform.forward = playerHead.forward;
        
        // Lock Rotation
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles = new Vector3(eulerAngles.x, 0, eulerAngles.z);
        transform.localRotation = Quaternion.Euler(eulerAngles);
>>>>>>> develop
    }
}
