using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
<<<<<<< HEAD
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();

    public GameObject gameLogic;

    public GameObject localPlayerPrefab;
    public GameObject networkPlayerPrefab;
    public GameObject itemSpawnerPrefab;
=======
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    

    public GameObject gameLogic;

    public GameObject itemSpawnersParent;

    public GameObject localPlayerPrefab;
    public GameObject networkPlayerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;
>>>>>>> develop
    
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
<<<<<<< HEAD
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        } else {
=======
            // Spawn local player
            _player = Instantiate(localPlayerPrefab, _position, _rotation);

        } else {
            // Spawn network player
>>>>>>> develop
            _player = Instantiate(networkPlayerPrefab, _position, _rotation);
        }

        EnableSystems();

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

<<<<<<< HEAD
    public void EnableSystems() {
        // Enable GameLogic
        gameLogic.SetActive(true);
    }

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem) {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }
=======
    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem) {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation, itemSpawnersParent.transform);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int _id, Vector3 _position) {
        Debug.Log($"Projectile Id: {_id}");
        GameObject _projectile = Instantiate(projectilePrefab, _position, Quaternion.identity);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }

    public void EnableSystems() {
        // Enable GameLogic
        gameLogic.SetActive(true);
    }
>>>>>>> develop
}
