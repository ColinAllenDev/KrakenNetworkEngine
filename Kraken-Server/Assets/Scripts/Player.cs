using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    
    #region Networking
    public int id;
    public string username;
    #endregion

    #region Input Settings
    bool[] inputs;
    float[] axes;
    #endregion

    #region Health Settings
    public float health;
    public float maxHealth = 100f;
    #endregion

    #region Item Settings
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    #endregion

    #region Weapon Settings
    public Transform shootOrigin;
    public float throwForce = 600f;
    #endregion

    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;

        health = maxHealth;
    
        inputs = new bool[1];
        axes = new float[2];
    }

    /// <summary> Input received by ClientHandle is assigned to global variables </summary>
    /// <param name=_inputs> Array of inputs sent from the client apart from the input axes </param>
    /// <param name=_axes> Array of input axes sent from the client </param>
    /// <param name=_playerRotation> The rotation input of the player </param>
    public void SetInput(bool[] _inputs, float[] _axes, Quaternion _playerRotation) {
        inputs = _inputs;
        axes = _axes;
        
        GetComponent<PlayerMovement>().SetInput(inputs, axes);
        GetComponent<PlayerMovement>().SetRotation(_playerRotation);
    }

    
    public void Shoot(Vector3 _viewDirection, float _damage) {
        if (health <= 0) return;

        if(Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, Mathf.Infinity)) {
            if(_hit.collider.CompareTag("Player")) {
               // Apply damage to hit player
               _hit.collider.GetComponent<Player>().TakeDamage(_damage);

               // Get the data of player your shot
               float _hitHealth = _hit.collider.GetComponent<Player>().health;
               string _hitUsername = _hit.collider.GetComponent<Player>().username;
               
               if(_hitHealth <= 0) {
                   // Send kill data to client
                   ServerSend.PlayerDied(_hit.collider.GetComponent<Player>(), this);

                   // Debug
                   Debug.Log($"Player {username} killed Player {_hitUsername}!");
               } else {
                   // Debug
                   Debug.Log($"Player {username} hit Player {_hitUsername} for {_damage} damage!");
               }
           }
       } 
    }

    public void ThrowItem(Vector3 _viewDirection) {
        if (health <= 0) return;
        
        if(itemAmount > 0) {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    /// <summary> Handles player taking damage and death </summary>
    /// <param name=_damage> The damage taken by the player </param> 
    public void TakeDamage(float _damage) {
        if(health <= 0) {
            return;
        }

        health -= _damage;
        if(health <= 0) {          
            // Set health to 0
            health = 0f;

            // Let clients know who died
            ServerSend.Message($"Player {username} has died!");

            // Start respawn process
            StartCoroutine(Respawn(5f));
        }

        // Update player health on client
        ServerSend.PlayerHealth(this);
    }

    /// <summary> The respawn methood </summary>
    public void RespawnPlayer() {
        // Start respawn process
        StartCoroutine(Respawn(5f));
    }

    /// <summary> The respawn coroutine </summary>
    /// <param name=_spawnTime> The time it takes to respawn </param>
    private IEnumerator Respawn(float _spawnTime) {       
        yield return new WaitForSeconds(_spawnTime);
        
        // Respawn point
        List<Transform> _playerSpawns = NetworkManager.instance.playerSpawns;
        int _spawnIndex = Random.Range(0, _playerSpawns.Count);
        
        // Update player position on client
        transform.position = _playerSpawns[_spawnIndex].position;

        // Send position data to client
        ServerSend.PlayerPosition(this);

        // Set health back to max
        health = maxHealth; 
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem() {
        if(itemAmount >= maxItemAmount) return false;
        
        itemAmount++;
        return true;
    }
}