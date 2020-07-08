using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float mouseSensitivity = 500f;

    public Transform camTransform;
    public Transform playerChest;

    float verticalRotation;
    float horizontalRotation;

    void Start() {
        // Lock Cursor
        ToggleCursorMode();

        verticalRotation = transform.eulerAngles.y;
        horizontalRotation = transform.eulerAngles.x;
    }

    void Update() {
        // Cursor Locking
        if(Input.GetKeyDown(KeyCode.Escape)) {
            ToggleCursorMode();
        }

        // Shooting Logic
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            ClientSend.PlayerShoot(camTransform.forward);
        }

        // Throwing logic
        if(Input.GetKeyDown(KeyCode.E)) {
            ClientSend.PlayerThrowItem(camTransform.forward);
        }
    }

    void FixedUpdate() {
        SendInputToServer();
    }

    void LateUpdate() {    
        if(Cursor.lockState == CursorLockMode.Locked) {
            Look();
        }

        // Debug Stuff
        Debug.DrawRay(camTransform.position, camTransform.forward * 2, Color.red);
    }

    void Look() {
        // Rotation Logic
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        
        verticalRotation += mouseInput.y * mouseSensitivity * Time.deltaTime;
        horizontalRotation += mouseInput.x * mouseSensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 55f);

        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
        playerChest.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // Added InputManager class
    private void SendInputToServer() {
        // Right now there is just keyboard input
        bool[] _inputs = InputManager.GetKeyboardInput();
        float[] _axes = InputManager.GetKeyboardAxis();

        ClientSend.PlayerMovement(_inputs, _axes);
    }

    private void ToggleCursorMode() {
        Cursor.visible = !Cursor.visible;

        if(Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
