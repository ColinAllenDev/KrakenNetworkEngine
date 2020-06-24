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
    public InputField usernameField;
    public InputField ipAddressField;
    
    
    // Debug UI
    public Text pingText;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start() {
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

    public void DisplayDebug() {
        // Display client ping
        pingText.text = ($"Ping: {(GameLogic.instance.RTT * 1000).ToString("f0")}ms");
    }

    public void ConnectToServer() {        
        // Change IP Address
        if(ipAddressField.text != "localhost")
        client.ip = ipAddressField.text;
        
        // Close connection menu
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ipAddressField.interactable = false;

        // Connect to server
        Client.instance.ConnectToServer();
    }
}
