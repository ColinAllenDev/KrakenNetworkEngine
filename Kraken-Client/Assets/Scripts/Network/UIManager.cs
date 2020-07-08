using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Client client;
<<<<<<< HEAD
    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipAddressField;
    public Text pingText;
    public Text clientTick;
=======
    
    // Start Menu
    private List<InputField> fields;
    public Button connectBtn;
    public GameObject startMenu;
    public GameObject HUD;
    public InputField usernameField;
    public InputField ipAddressField;
    
    // Messages
    public Text deathText;
>>>>>>> develop

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

<<<<<<< HEAD
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

=======
    void Start() {
        // Disable HUD
        HUD.SetActive(false);
        deathText.gameObject.SetActive(false);

        // List input fields
        fields = new List<InputField>{usernameField, ipAddressField};
    }

    int _fieldIndex;
    void Update() {
        // Tab through input fields
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if(fields.Count <= _fieldIndex) {
                _fieldIndex = 0;
            }
            fields[_fieldIndex].Select();
            _fieldIndex++;
        }

        // Connect when pressing "Enter"
        if(Input.GetKeyDown(KeyCode.Return) && connectBtn.interactable) {
            ConnectToServer();
        }
    }

    public void CheckInput() {
        if(usernameField.text != "" && ipAddressField.text != "") {
            connectBtn.interactable = true;
        } else {
            connectBtn.interactable = false;
        }
    }

    // TODO: Refactor this
    public void DisplayDeathText(string _username) {
        // Start respawn countdown and display death text
        StartCoroutine(RespawnTimer(5.0f, _username));
    }

    float currentTime;
    IEnumerator RespawnTimer(float _time, string _username) {
        // Enable death text
        deathText.text = ($"Killed By \n{_username}\n\nRespawning...\n5");
        deathText.gameObject.SetActive(true);

        // Countdown logic
        currentTime = _time;
        while(currentTime > 0) {
            yield return new WaitForSeconds(1.0f);
            currentTime--;
            deathText.text = ($"Killed By \n{_username}\n\nRespawning...\n{currentTime}");
        }
        
        // Disable death text
        deathText.gameObject.SetActive(false);
    }

    public void ConnectToServer() {        
        // Change IP Address
        if(ipAddressField.text != "localhost")
        client.ip = ipAddressField.text;
        
>>>>>>> develop
        // Close connection menu
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ipAddressField.interactable = false;

<<<<<<< HEAD
=======
        // Display HUD
        HUD.SetActive(true);

>>>>>>> develop
        // Connect to server
        Client.instance.ConnectToServer();
    }
}
