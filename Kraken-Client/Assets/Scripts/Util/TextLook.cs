using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLook : MonoBehaviour
{
    public PlayerController player;

    void Update()
    {
        if(player == null) return;
        Vector3 lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
    }
}
