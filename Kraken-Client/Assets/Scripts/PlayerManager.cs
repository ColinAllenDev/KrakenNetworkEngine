using System;
using UnityEngine;

/// <summary> Handles the player's states and attributes </summary>
public class PlayerManager : MonoBehaviour
{
    #region Network Settings
    public int id;
    public string username;
    #endregion
    
    #region Player Attributes
    public float health;
    public float maxHealth = 100f;
    public int itemCount = 0;
    public Vector3 velocity;
    #endregion

    #region Player States
    public enum PlayerState {
        Alive,
        Dead
    }
    public PlayerState state = PlayerState.Alive;
    #endregion

    /// <summary> Initialize our player </summary>
    /// <param name=_id> The id of our player </param>
    /// <param name=_username> The username of our player </param>
    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;
        health = maxHealth;
        velocity = Vector3.zero;
    }

    /// <summary> Set the health of our player </summary>
    /// <param name=_health> The health to set </param>
    public void SetHealth(float _health) {
        health = _health;
        if(health <= 0f) {
            Die();
        }
    }

    /// <summary> Set the velocity of our player </summary>
    /// <param name=_velocity> The velocity to set </param>
    public void SetVelocity(Vector3 _velocity) {
        velocity = _velocity;
    }

    /// <summary> The player's death event </summary>
    public void Die() {
        // TODO: Death system
        state = PlayerState.Dead;        
    }

    /// <summary> The player's respawn event </summary>
    public void Respawn() {
        // TODO: Respawn system
        state = PlayerState.Alive;
        
        SetHealth(maxHealth);
    }
}
