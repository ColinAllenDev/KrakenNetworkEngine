using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour {
    void Update() {
        Vector3 eulerAngles = transform.localEulerAngles;
        eulerAngles = new Vector3(eulerAngles.x, 0, eulerAngles.z);
        transform.localRotation = Quaternion.Euler(eulerAngles);
    }
}
