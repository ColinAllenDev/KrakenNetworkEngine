using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    PlayerManager player;
    public float sensitivity = 100f;
    public float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    Transform cameraTarget;
    Transform playerChest;

    public bool enableBobbing = false;

    private void Awake() {
        player = GameObject.FindObjectOfType<PlayerManager>();
        cameraTarget = GameObject.FindWithTag("CameraTarget").transform;
        playerChest = GameObject.FindWithTag("PlayerChest").transform;
    }

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
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void FixedUpdate() {
        transform.position = cameraTarget.position;
    }

    private void LateUpdate() {
        playerChest.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        float _mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }
/*
    public float idleAmplitude = 0.01f;
    public float idlePeriod = 0.55f;
    public float walkAmplitude = 0.04f;
    public float walkPeriod = 0.075f;
    public float crouchPeriod = 0.09f;
    private void HeadBob() {
        float theta = Time.timeSinceLevelLoad / idlePeriod;
        float distance = idleAmplitude * Mathf.Sin(theta);
        float idleMag = 3f, walkMag = 2f;
        float movePeriod = walkPeriod;
        // Change magnitude
        idleMag = 3f; walkMag = 2f;
        movePeriod = walkPeriod;

        // Change bob speed based on movement
        /*
        if(Vector3.velocity.magnitude < idleMag) {
            theta = Time.timeSinceLevelLoad / idlePeriod;
            distance = idleAmplitude * Mathf.Sin(theta);
        } else if (playerBody.velocity.magnitude > walkMag) {
            theta = Time.timeSinceLevelLoad / movePeriod;
            distance = walkAmplitude * Mathf.Sin(theta);
        }
        
        transform.position = transform.position + Vector3.up * distance;   
    }
*/

    private void ToggleCursorMode() {
        Cursor.visible = !Cursor.visible;

        if(Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }


    }
}
