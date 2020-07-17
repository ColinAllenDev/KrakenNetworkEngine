using UnityEngine;

public class WP_Pistol : Weapon, IShootable {
    public void Shoot(Vector3 _facing) {
        // Pistol Shooting
        Debug.Log($"Player just shot a {weaponName}");

        // Current shooting logic
        if(Physics.Raycast(transform.position, _facing, out RaycastHit _hit, Mathf.Infinity)) {
            if(_hit.collider.CompareTag("Player")) {
               // Get both players
               Player _player = GetComponentInParent<Player>();
               Player _hitPlayer = _hit.collider.GetComponent<Player>();
               
               // Apply damage to hit player
               _hitPlayer.TakeDamage(maxDamage); // TODO: Add damage system

               // Get the data of player your shot
               float _hitHealth = _hitPlayer.health;
               string _hitUsername = _hitPlayer.username;
               
               if(_hitHealth <= 0) {
                   // Send kill data to client
                   ServerSend.PlayerDied(_hitPlayer, _player);

                   // Debug
                   Debug.Log($"Player {_player.username} killed Player {_hitUsername}!");
               } else {
                   // Debug
                   Debug.Log($"Player {_player.username} hit Player {_hitUsername} for {maxDamage} damage!");
               }
           }
       }
    }
}
