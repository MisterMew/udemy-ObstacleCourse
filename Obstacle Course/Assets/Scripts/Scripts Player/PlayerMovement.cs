using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* Variables */
    private Rigidbody rb = default;

    /* Input Variables */
    private Vector3 input = Vector3.zero;

    /* Ground Check Variables */
    [Header("GROUND CHECK Config")]
    public bool IsGrounded = false;

    /* Movement Variables*/
    [Header("MOVEMENT Config")]
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float moveAcceleration = 0f;
    [SerializeField] private float sprintSpeed = 0f;

    /* Rotation Variables */
    [Header("ROTATION Config")]
    [SerializeField] private float rotationSpeed = 0f;

    /* Jump Variables */
    [Header("JUMP Config")]
    [SerializeField] private float jumpForce = 0f;


    /* VFX Variables */
    [Header("VFX Particles")]
    public ParticleSystem jumpParticles;



    /* FUNCTIONS */
    void Awake() => rb = GetComponent<Rigidbody>();

    // Update is called once per frame
    void Update()
    {
        InitInputs();

        GroundCheck();
    }

    void FixedUpdate()
    {
        HandleMovement();

        HandleFacingDirection();

        HandleJump();

    }


    /* METHODS */
    private void InitInputs()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");
        input.z = Input.GetAxis("Vertical");
    }

    private void GroundCheck()
    {

    }

    private void HandleMovement()
    {
        var acceleration = IsGrounded ? moveAcceleration : moveAcceleration * 0.5f;
        acceleration *= Time.fixedDeltaTime;

        // Input Left/Right
        if (Input.GetAxis("Horizontal") < 0)
        {
            if (rb.velocity.x > 0f) input.x = 0f; //Immediately stop
            input.x = Mathf.MoveTowards(input.x, -1, acceleration); //Move in direction
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            if (rb.velocity.x < 0f) input.x = 0f;
            input.x = Mathf.MoveTowards(input.x, 1, acceleration);
        }

        // Input Forward/Backward
        if (Input.GetAxis("Vertical") < 0)
        {
            if (rb.velocity.z > 0f) input.z = 0f;
            input.z = Mathf.MoveTowards(input.z, -1, acceleration);
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            if (rb.velocity.z < 0f) input.z = 0f;
            input.z = Mathf.MoveTowards(input.z, 1, acceleration);
        }

        rb.velocity = Vector3.MoveTowards(rb.velocity, input * moveSpeed, 100 * Time.fixedDeltaTime);
    }

    /// <summary>
    /// CURRENTLY CANT ROTATE AND JUMP CORRECTLY
    /// ROTATION PREVENTS JUMPING????
    /// </summary>
    private void HandleFacingDirection()
    {
        if (input != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input, Vector3.up);
            targetRotation.x = 0;
            targetRotation.z = 0;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleJump()
    {
        if (Input.GetAxis("Jump") > 0)
        {
            rb.velocity = Vector3.up * jumpForce * Time.deltaTime;
        }
    }

    private void CreateParticles()
    {
        jumpParticles.Play();
    }

    //FEATURE: SPRINT
    // Collect slime balls to increase in size (to a pre-defined maximum point)
    // Sprinting consumes size but has a huge speed boost
    // Sprinting emits a unique trail:
    /// trailRenderer.emitting true/false

    //FEATURE: JUMP
    // Jumping uses no cost
    // Double jumps consume size

    //FEATURE: WALL STICK
    // Stick to walls, can slide down, can jump off (also resets double jump?)

    //SIZING
    // Consuming size emitts an echo effect
    // Gaining size emits a pulse effect
}
