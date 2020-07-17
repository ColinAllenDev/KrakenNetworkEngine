using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    
    #region Networking
    [Header("Network Settings")]
    public int id;
    public string username;
    #endregion

    #region Input Settings
    [Header("Input Settings")]
    bool[] inputs;
    float[] axes;
    #endregion

    #region Health Settings
    [Header("Health Settings")]
    public float health;
    public float maxHealth = 100f;
    #endregion

    #region Item Settings
    [Header("Item Settings")]
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    #endregion

    #region Weapon Settings
    [Header("Weapon Settings")]
    public Transform loadout;
    private Weapon currentWeapon;

    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;
    public Weapon meleeWeapon;
    public Transform shootOrigin; // Temporary
    #endregion

    public void Initialize(int _id, string _username) {
        // Network
        id = _id;
        username = _username;

        // Health
        health = maxHealth;
    
        // Input
        inputs = new bool[1];
        axes = new float[2];

        // Weapons
        currentWeapon = primaryWeapon;
        currentWeapon.weaponState = Weapon.WeaponState.Equipped;
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

    public void InitializeLoadout(int _toClient) {
        Weapon _primary = Instantiate(primaryWeapon, transform.position, Quaternion.identity, loadout);
        Weapon _secondary = Instantiate(secondaryWeapon, transform.position, Quaternion.identity, loadout);
        Weapon _melee = Instantiate(meleeWeapon, transform.position, Quaternion.identity, loadout);

        primaryWeapon = _primary;
        secondaryWeapon = _secondary;
        meleeWeapon = _melee;

        primaryWeapon.ownerId = id;
        secondaryWeapon.ownerId = id;
        meleeWeapon.ownerId = id;
        
        ServerSend.PlayerLoadout(_toClient, this);
    }

    public void ThrowItem(Vector3 _viewDirection) {
        if (health <= 0) return; // TODO: Dead and Alive state management
        
        if(itemAmount > 0) {
            itemAmount--;
            // TODO: Move to grenade item class
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, 5f, id);
        }
    }

    /// <summary> Shoot the currently equipped weapon </summary>
    /// <param name=_facing> The direction the player is facing </param>
    public void Shoot(Vector3 _facing) {
        currentWeapon.GetComponent<IShootable>().Shoot(_facing);
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