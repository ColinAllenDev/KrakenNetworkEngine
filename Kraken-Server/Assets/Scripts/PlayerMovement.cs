using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    #region Components
    [HideInInspector] public Rigidbody playerBody;
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
    [SerializeField, Range(0f, 100f)] float maxSpeed = 8f;
    [SerializeField, Range(0f, 100f)] float maxAcceleration = 20f, maxAirAcceleration = 5f;
    public float jumpHeight = 1.5f;
    public int maxAirJumps = 1;
    int jumpPhase;
    Vector3 velocity, targetVelocity;
    #endregion

    #region Grounding Settings
    [Header("Grounding Settings")]
    [SerializeField, Range(0,90)] float maxGroundAngle = 46f, maxStairAngle = 50f;
    [SerializeField, Range(0,90)] float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)] float probeDistance = 1.5f;
    [SerializeField] LayerMask probeMask = -1, stairMask = -1;
    int stepsSinceLastGrounded, stepsSinceLastJump;
    float minGroundDotProduct, minStairDotProduct;
    int groundContactCount, steepContactCount;
    bool isGrounded => groundContactCount > 0;
    bool isSteep => steepContactCount > 0;
    Vector3 contactNormal, steepNormal;
    #endregion

    #region Getters
    public Vector3 getVelocity {
        get { return playerBody.velocity; }
    }
    #endregion

    void Awake() {
        // Get components
        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerOrientation = transform.GetChild(0);

        // Validate dot product
        OnValidate();

        // Initialize input
        inputs = new bool[1];
        axes = new float[2];
        moveInput = new Vector2();
    }

    /// <summary> Validates the dot product of our max ground and stair angles </summary> 
    void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairDotProduct = Mathf.Cos(maxStairAngle *Mathf.Deg2Rad);
    }

    void Update() {
        // Get user input
        GetInput();
    }

    void FixedUpdate() {
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

    /// <summary> Updates player grounded state </summary>
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

    /// <summary> Clears player grounded state </summary>
    void ClearState() {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
    }

    /// <summary> The main Player classes' input data </summary>
    /// <param name=_inputs> Player's Array of inputs </param>
    /// <param name=_axes> Player's Array of input axes sent from the client </param>
    public void SetInput(bool[] _inputs, float[] _axes) {
        inputs = _inputs;
        axes = _axes;   
    }
    
    /// <summary> Set the rotation of the player </summary>
    /// <param name=_playerRotation> The rotation of the player </param>
    public void SetRotation(Quaternion _playerRotation) {
        transform.rotation = _playerRotation;
    }

    /// <summary> Movement input is processed here </summary>
    void GetInput() {       
        // Keyboard input
        moveInput = new Vector2(axes[0], axes[1]);
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        jumpInput |= inputs[0];

        // Get target velocity
        targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * maxSpeed;
    }

    /// <summary> Adjust player velocity based on contact slope </summary>
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

    /// <summary> Handle player jumping logic </summary>
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

    /// <summary> Get the minimum dot-product </summary>
    /// <param name=layer> The index of the layer we are colliding with </param>
    /// <return> Returns the minimum dot product of our contact layer </return>
    float GetMinDot(int layer) {
        return (stairMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairDotProduct;
    }

    /// <summary> Snaps the player to the ground to prevent flying off slopes </summary>
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

    /// <summary> Helper methood that fixes issues with multiple steep contacts </summary>
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

    /// <summary> Gets a vector projection of the contact plane </summary>
    /// <param name=vector> The vector being projected </param>
    /// <return> Returns a the contact normal of the plane </return>
    Vector3 ProjectOnContactPlane(Vector3 vector) {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    /// <summary> Unity's OnCollisionEnter method </summary>
    void OnCollisionEnter(Collision collision) {
        EvaluateCollision(collision);
    }
    
    /// <summary> Unity's OnCollisionStay method </summary>
    void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }
    
    /// <summary> Evaluates collisions to prevent getting stuck on walls </summary>
    /// <param name=collision> The collision data </param>
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

    /// <summary> Sends player movement data from the client to the sever </summary>
    void SendPlayer() {
        Player _player = GetComponent<Player>();
        ServerSend.PlayerPosition(_player);
        ServerSend.PlayerRotation(_player);
        ServerSend.PlayerVelocity(_player);
    }
}