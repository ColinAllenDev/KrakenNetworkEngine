using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
    public float lerpRate = 0.5f;

    public int id;
    public string username;
    public float health;
    public float maxHealth = 100f;
    public int itemCount = 0;
    public Vector3 velocity;

    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    public void SetHealth(float _health) {
        health = _health;
        if(health <= 0f) {
            Die();
        }
    }

    public void SetVelocity(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void Die() {
    // TODO: Change death system
    //    model.enabled = false;
    }

    public void Respawn() {
    // TODO: Change respawn system
    //    model.enabled = true;
        SetHealth(maxHealth);
    }
}
