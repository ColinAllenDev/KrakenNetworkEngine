using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles the player's input, rotation, and behavior </summary>
public class PlayerController : MonoBehaviour
{    
    #region Components
    private PlayerManager player;
    public Transform playerCamera;
    public Transform playerChest;
    #endregion

    #region Input Settings
    Vector2 mouseInput;
    bool inputLocked = false;
    #endregion

    #region Rotation Settings
    public float mouseSensitivity = 500f;   
    float verticalRotation;
    float horizontalRotation;
    #endregion


    void Start() {
        // Components
        player = GetComponent<PlayerManager>();
        
        // Lock Cursor
        ToggleCursorMode();

        // Initialize Rotation
        verticalRotation = transform.eulerAngles.y;
        horizontalRotation = transform.eulerAngles.x;
    }

    void Update() {
        // Cursor Locking
        if(Input.GetKeyDown(KeyCode.Escape)) {
            ToggleCursorMode();
        }

        // Check player state
        if(player.state == PlayerManager.PlayerState.Dead) return;

        // Mouse input
        if(!inputLocked) {
            GetMouseInput();
        }

        // Shooting Logic
        if(Input.GetKeyDown(KeyCode.Mouse0) && !inputLocked) {
            ClientSend.PlayerShoot(playerCamera.forward, 25f);
        }

        // Throwing logic
        if(Input.GetKeyDown(KeyCode.E) && !inputLocked) {
            ClientSend.PlayerThrowItem(playerCamera.forward);
        }
    }

    void FixedUpdate() {
        // Check player state
        if(player.state == PlayerManager.PlayerState.Dead) return;

        SendInputToServer();
    }

    void LateUpdate() {    
        // Check player state
        if(player.state == PlayerManager.PlayerState.Dead) return;
        
        if(Cursor.lockState == CursorLockMode.Locked) {
            PlayerRotation();
            ChestRotation();
        }
    }

    // Mouse Input
    void GetMouseInput() {
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
    }

    // Player controller rotation
    void PlayerRotation() {
        horizontalRotation += mouseInput.x * mouseSensitivity * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }

    // Player chest rotation
    void ChestRotation() {
        verticalRotation += mouseInput.y * mouseSensitivity * Time.deltaTime;
        
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 55f);

        playerChest.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    
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
