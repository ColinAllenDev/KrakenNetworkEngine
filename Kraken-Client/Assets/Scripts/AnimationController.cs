using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    PlayerManager player;
    Animator animController;

    void Start() {
        player = GetComponent<PlayerManager>();
        animController = GetComponentInChildren<Animator>();
    }

    void Update() {
        float _velocity = player.velocity.magnitude;

        animController.SetFloat("_velocity", _velocity);
    }
}
