using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Client client;
    
    #region Start Menu
    [Header("Start Menu")]
    public GameObject startMenu;
    public Button connectBtn;
    public List<InputField> inputFields;
    #endregion

    #region Loadout Menu
    [Header("Loadout Menu")]
    public GameObject loadoutMenu;
    #endregion

    #region HUD
    [Header("HUD")]
    public GameObject HUD;
    #endregion

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
    }

    int _fieldIndex;
    void Update() {
        // Tab through input fields
        if(Input.GetKeyDown(KeyCode.Tab) && inputFields[0] && inputFields[1]) {
            if(inputFields.Count <= _fieldIndex) {
                _fieldIndex = 0;
            }
            inputFields[_fieldIndex].Select();
            _fieldIndex++;
        }

        // Connect when pressing "Enter"
        if(Input.GetKeyDown(KeyCode.Return) && connectBtn.interactable) {
            ConnectToServer();
        }
    }

    public void CheckInput() {
        if(inputFields[0].text != "" && inputFields[1].text != "") {
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
        if(inputFields[0].text != "localhost")
        client.ip = inputFields[0].text;
        
        // Close connection menu
        startMenu.SetActive(false);
        inputFields[0].interactable = false;
        inputFields[1].interactable = false;

        // Display HUD
        HUD.SetActive(true);

        // Connect to server
        Client.instance.ConnectToServer();
    }
}
