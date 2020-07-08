using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rigidBody;
    public int thrownByPlayer;
    public Vector3 initialForce;
    public float explosionRadius = 1.5f;
    public float explosionDamage = 75f;
    public float explosionTime = 10f;

    void Start() {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        ServerSend.SpawnProjectile(this, thrownByPlayer);

        rigidBody.AddForce(initialForce);
        StartCoroutine(ExplodeAfterTime(explosionTime));
    }

    void FixedUpdate() {
        ServerSend.ProjectilePosition(this);
    }

    void OnCollisionEnter(Collision collision) {
        // Make sure projectile is not colliding with player throwing it
        if(collision.collider.CompareTag("Player")) {
            int _playerId = collision.collider.GetComponent<Player>().id;
            if(_playerId == thrownByPlayer) return;
        }

        // Explode on any other surface
        Explode();
    }

    public void Initialize(Vector3 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer) {
        initialForce = _initialMovementDirection * _initialForceStrength;
        thrownByPlayer = _thrownByPlayer;
    }

    void Explode() {
        ServerSend.ProjectileExploded(this);

        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider _collider in _colliders) {
            if(_collider.CompareTag("Player")) {
               _collider.GetComponent<Player>().TakeDamage(explosionDamage);
            }
        } 
        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime(float time) {
        yield return new WaitForSeconds(time);

        Explode();
    }
}
