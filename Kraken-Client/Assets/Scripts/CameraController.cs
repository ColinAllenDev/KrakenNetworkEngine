using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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
    }
}
