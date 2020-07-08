using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Client client;
    
    // Start Menu
    private List<InputField> fields;
    public Button connectBtn;
    public GameObject startMenu;
    public GameObject HUD;
    public InputField usernameField;
    public InputField ipAddressField;
    
    // Messages
    public Text deathText;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

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
        
        // Close connection menu
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ipAddressField.interactable = false;

        // Display HUD
        HUD.SetActive(true);

        // Connect to server
        Client.instance.ConnectToServer();
    }
}
