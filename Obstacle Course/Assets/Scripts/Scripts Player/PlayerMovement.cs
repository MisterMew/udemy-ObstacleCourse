using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* Variables */
    private Rigidbody rb;
    [SerializeField] private float jumpVelocityFalloff = 0F;
    [SerializeField] private float fallMultiplier = 0F;

    /* Movement Variables */
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float jumpHeight = 0f;
    private float xValue, yValue, zValue = 0;

    /* VFX Variables */
    public ParticleSystem particle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        xValue = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        yValue = Input.GetAxis("Jump") * jumpHeight * Time.deltaTime;
        zValue = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(xValue, yValue, zValue);

        if (Input.GetButton("Jump")) CreateParticles();

        if (rb.velocity.y < jumpVelocityFalloff || rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
    }

    private void CreateParticles()
    {
        particle.Play();
    }

    //FEATURE: SPRINT
    // Collect slime balls to increase in size (to a pre-defined maximum point)
    // Sprinting consumes size but has a huge speed boost
    // Sprinting emits a unique trail:
    /// trailRenderer.emitting true/false

    //FEATURE: JUMP
    // Jumping uses no cost
    // Double jumps consume size

    //SIZING
    // Consuming size emitts an echo effect
    // Gaining size emits a pulse effect
}
