using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Client client;
    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipAddressField;
    public Text pingText;
    public Text clientTick;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void DisplayPing() {
        // Display client ping
        pingText.text = ($"Ping: {(GameLogic.instance.RTT * 1000).ToString("f0")}ms");
    }

    public void DisplayTick() {
        // Display client tick
        StartCoroutine(Tick(0.01f));
    }

    private IEnumerator Tick(float delay) {
        while(true) {
            yield return new WaitForSeconds(delay);
            clientTick.text = ($"Tick: {GameLogic.instance.tick}");
        }
    }

    public void ConnectToServer() {
        // Change IP Address
        if(ipAddressField.text != "") {
            client.ip = ipAddressField.text;
        }

        // Close connection menu
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ipAddressField.interactable = false;

        // Connect to server
        Client.instance.ConnectToServer();
    }
}
