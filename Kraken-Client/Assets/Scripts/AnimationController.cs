using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    PlayerManager player;
    Animator animController;

    float animVelocity;

    void Awake() {
        player = GetComponent<PlayerManager>();
        animController = GetComponentInChildren<Animator>();
    }

    void FixedUpdate() {
        animVelocity = player.velocity.magnitude;
        SetAnim_Velocity(animVelocity);
    }

    public void SetAnim_Velocity(float _velocity) {
        animController.SetFloat("_velocity", _velocity);
    }
}
