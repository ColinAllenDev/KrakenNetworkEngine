using System;
using UnityEngine;

public class ServerLogic : MonoBehaviour {
    public static ServerLogic instance;

    public int tick = 0;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exitsts, destroying gameobject!");
            Destroy(this);
        }
    }

    private void FixedUpdate() {
        tick++;
    }

}