using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    
    #region Networking
    public int id;
    public string username;
    #endregion

    #region Components
    Rigidbody playerBody;
    CapsuleCollider playerCollider;
    Transform playerOrientation;
    #endregion

    #region Input Settings
    bool[] inputs;
    float[] axes;
    Vector2 moveInput;
    bool jumpInput;
    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 20f, maxAirAcceleration = 5f;
    public float jumpHeight = 2.5f;
    public int maxAirJumps = 1;
    int jumpPhase;
    Vector3 velocity, targetVelocity;
    #endregion

    #region Grounding Settings
    [Header("Grounding Settings")]
    [SerializeField, Range(0,90)] float maxGroundAngle = 45f, maxStairAngle = 50f;
    [SerializeField, Range(0,90)] float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)] float probeDistance = 1f;
    [SerializeField] LayerMask probeMask = -1, stairMask = -1;
    int stepsSinceLastGrounded, stepsSinceLastJump;
    float minGroundDotProduct, minStairDotProduct;
    int groundContactCount, steepContactCount;
    bool isGrounded => groundContactCount > 0;
    bool isSteep => steepContactCount > 0;
    Vector3 contactNormal, steepNormal;
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

    #region Getters
    public Vector3 getVelocity {
        get { return playerBody.velocity; }
    }
    #endregion

    void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairDotProduct = Mathf.Cos(maxStairAngle *Mathf.Deg2Rad);
    }

    public void Initialize(int _id, string _username) {
        id = _id;
        username = _username;

        health = maxHealth;
    
        inputs = new bool[1];
        axes = new float[2];
    }

    void Awake() {
        // Get components
        playerBody = this.GetComponent<Rigidbody>();
        playerCollider = this.GetComponent<CapsuleCollider>();
        playerOrientation = this.transform.GetChild(0);

        // Validate dot product
        OnValidate();
    }

    void Update() {
        // Get user input
        GetInput();
    }

     void FixedUpdate() {
        if(health <= 0) return;

        // Update player state
        UpdateState();

        // Move player
        AdjustVelocity();

        // Jumping
        if(jumpInput) {
            jumpInput = false;
            Jump();
        }

        // Set velocity
        playerBody.velocity = velocity;

        // Send position to clinet
        SendPlayer();

        // Update ground normal
        ClearState();    
    }

    void UpdateState() {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;

        velocity = playerBody.velocity;
        
        if(isGrounded || SnapToGround() || CheckSteepContacts()) {
            stepsSinceLastGrounded = 0;
            if(stepsSinceLastJump > 1) {
                jumpPhase = 0;
            }
            if(groundContactCount > 1) {
                contactNormal.Normalize();
            }
        } else {
            contactNormal = Vector3.up;
        }

    }

    void ClearState() {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
    }

    public void SetInput(bool[] _inputs, float[] _axes, Quaternion _rotation) {
        inputs = _inputs;
        axes = _axes;
        transform.rotation = _rotation;
    }

    void GetInput() {
        // Keyboard input
        moveInput = new Vector2(axes[0], axes[1]);
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        jumpInput |= inputs[0];

        // Get target velocity
        targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * maxSpeed;
    }

    void AdjustVelocity() {
        Vector3 _xAxis = ProjectOnContactPlane(playerOrientation.right).normalized;
        Vector3 _zAxis = ProjectOnContactPlane(playerOrientation.forward).normalized;

        float _currentX = Vector3.Dot(velocity, _xAxis);
        float _currentZ = Vector3.Dot(velocity, _zAxis);

        float _acceleration = isGrounded ? maxAcceleration : maxAirAcceleration;
        float _maxSpeedChange = _acceleration * Time.deltaTime;

        float _newX = Mathf.MoveTowards(_currentX, targetVelocity.x, _maxSpeedChange);
        float _newZ = Mathf.MoveTowards(_currentZ, targetVelocity.z, _maxSpeedChange);

        velocity += _xAxis * (_newX - _currentX) + _zAxis * (_newZ - _currentZ);
    }

    void Jump() {
        Vector3 _jumpDirection;

        if(isGrounded) {
            _jumpDirection = contactNormal;
        } else if (isSteep) {
            _jumpDirection = steepNormal;
            jumpPhase = 0;
        } else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
            if(jumpPhase == 0) {
                jumpPhase = 1;
            }
            _jumpDirection = contactNormal;
        } else {
            return;
        }

        stepsSinceLastJump = 0;
        jumpPhase += 1;
        _jumpDirection = (_jumpDirection + Vector3.up).normalized;
        float _jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        float _alignedSpeed = Vector3.Dot(velocity, _jumpDirection);
        if(_alignedSpeed > 0f) {
            _jumpSpeed = Mathf.Max(_jumpSpeed - _alignedSpeed, 0f);
        }

        velocity += _jumpDirection * _jumpSpeed;
    }

    float GetMinDot(int layer) {
        return (stairMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairDotProduct;
    }

    bool SnapToGround() {
        if(stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
            return false;
        }

        float _speed = velocity.magnitude;
        if(_speed > maxSnapSpeed) {
            return false;
        }

        if(!Physics.Raycast(playerBody.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask)) {
            return false;
        }

        if(hit.normal.y < GetMinDot(hit.collider.gameObject.layer)) {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        float _dot = Vector3.Dot(velocity, hit.normal);
        if(_dot > 0f) {
            velocity = (velocity - hit.normal * _dot).normalized * _speed;
        }
    
        return true;
    }

    bool CheckSteepContacts() {
        if(steepContactCount > 1) {
            steepNormal.Normalize();
            if(steepNormal.y >= minGroundDotProduct) {
                groundContactCount += 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }

    Vector3 ProjectOnContactPlane(Vector3 vector) {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    void OnCollisionEnter(Collision collision) {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision) {
        float minDot = GetMinDot(collision.gameObject.layer);
        for(int i = 0; i < collision.contactCount; i++) {
            Vector3 _normal = collision.GetContact(i).normal;
            if(_normal.y >= minDot) {
                groundContactCount += 1;
                contactNormal += _normal;
            } else if(_normal.y > -0.01f) {
                steepContactCount += 1;
                steepNormal += _normal;
            }
        }
    }

    void SendPlayer() {
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
        ServerSend.PlayerVelocity(this);
    }

    public void Shoot(Vector3 _viewDirection) {
        if (health <= 0) return;

        if(Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f)) {
            if(_hit.collider.CompareTag("Player")) {
               _hit.collider.GetComponent<Player>().TakeDamage(50f);
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

    public void TakeDamage(float _damage) {
        if(health <= 0) {
            return;
        }

        health -= _damage;
        if(health <= 0) {
            health = 0f;
            // TODO: Kill player
            // New spawn point
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }
        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn() {
        yield return new WaitForSeconds(5f);
        health = maxHealth;
        
        // TODO: Kill player
        
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem() {
        if(itemAmount >= maxItemAmount) return false;
        
        itemAmount++;
        return true;
    }
}