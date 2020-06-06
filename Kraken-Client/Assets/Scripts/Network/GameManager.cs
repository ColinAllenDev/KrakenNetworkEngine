﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();

    public GameObject gameLogic;

    public GameObject localPlayerPrefab;
    public GameObject networkPlayerPrefab;
    public GameObject itemSpawnerPrefab;
    
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation) {
        GameObject _player;
        if(_id == Client.instance.myId) {
            // Spawn local player
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
            
            // Apply username to local player
            _player.GetComponentInChildren<TextMesh>().text = _username;
        } else {
            // Spawn network player
            _player = Instantiate(networkPlayerPrefab, _position, _rotation);
            
            // Apply username to network player
            _player.GetComponentInChildren<TextMesh>().text = _username;
        }

        EnableSystems();

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void EnableSystems() {
        // Enable GameLogic
        gameLogic.SetActive(true);
    }

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem) {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }
}
