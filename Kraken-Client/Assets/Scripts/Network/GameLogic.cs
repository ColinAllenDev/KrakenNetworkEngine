using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {
    public static GameLogic instance;

    public int tick = 0;
    public int delayTick;
    public float secPerTick = 0.033333f;
    public float RTT = 0; // ping

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Debug.Log("Instance already exists, destroying gameobject!");
            Destroy(this);
        }

        // Ping server
        ClientSend.Ping();
        StartCoroutine(Ping(5f));
    }

    private void FixedUpdate() {
        tick++; 
        delayTick = tick - 3;
        if(delayTick < 0) delayTick = 0;
    }

    private IEnumerator Ping(float delay) {
        while(true) {
            yield return new WaitForSeconds(delay);
            ClientSend.Ping();
        }
    }

}