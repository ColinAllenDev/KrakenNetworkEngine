using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform camTransform;

    private void Update() {      
        // Shooting Logic
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            ClientSend.PlayerShoot(camTransform.forward);
        }
    }

    private void FixedUpdate() {
        SendInputToServer();
    }

    // Added InputManager class
    private void SendInputToServer() {
        // Right now there is just keyboard input
        bool[] _inputs = InputManager.GetKeyboardInput();

        ClientSend.PlayerMovement(_inputs);
    }

}
