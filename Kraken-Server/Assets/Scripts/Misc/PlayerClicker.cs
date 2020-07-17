using UnityEngine;

public class PlayerClicker : MonoBehaviour {
    
    void Update() {
        Vector3 _input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float _moveSpeed = 2.5f;

        transform.position += (_input.x * transform.right + _input.z * transform.forward) * _moveSpeed * Time.deltaTime;
    }
}
