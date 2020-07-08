using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject playerPrefab;
    public GameObject projectilePrefab;
    public List<Transform> playerSpawns = new List<Transform>();

    private void Awake() {
        if(instance == null) {
            instance = this; 
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object.");
            Destroy(this);
        }
    }

    private void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        
        Server.Start(50, 26950);

    }

    private void OnApplicationQuit() {
        Server.Stop();
    }

    public Player InstantiatePlayer() {
        // Loop through spawn points
        int _spawnIndex = Random.Range(0, playerSpawns.Count);
        
        // Spawn player
        return Instantiate(playerPrefab, playerSpawns[_spawnIndex].position, playerSpawns[_spawnIndex].rotation).GetComponent<Player>();
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin) {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.5f, Quaternion.identity).GetComponent<Projectile>();
    }
}
