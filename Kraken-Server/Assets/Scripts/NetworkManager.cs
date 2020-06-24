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

    int spawnIndex = -1;
    public Player InstantiatePlayer() {
        // Iterate through spawn positions
        int spawnCount = playerSpawns.Count;
        if(spawnIndex <= spawnCount) {
            spawnIndex++;
        } else {
            spawnIndex = -1;
        }
        
        // Spawn player
        return Instantiate(playerPrefab, playerSpawns[spawnIndex].position, Quaternion.identity).GetComponent<Player>();
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin) {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.5f, Quaternion.identity).GetComponent<Projectile>();
    }
}
