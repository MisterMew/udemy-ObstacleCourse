﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    /* Variables */
    private Rigidbody rb;
    private FrameInputs inputs;

    /* Detection Variables */
    [Header("Detection Config")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _grounderOffset = -1, _grounderRadius = 0.2f;
    [SerializeField] private float _wallCheckOffset = 0.5f, _wallCheckRadius = 0.05f;
    private bool _isAgainstLeftWall, _isAgainstRightWall, _pushingLeftWall, _pushingRightWall;
    public bool IsGrounded;
    public static event Action OnTouchedGround;

    private readonly Collider[] ground = new Collider[1];


    /* Methods */
    private void Start() => rb = GetComponent<Rigidbody>();
    private void Update()
    {
        GatherInputs();

        //HandleGrounding();

        HandleWalking();

        HandleJumping();

        //HandleWallSlide();

        //HandleWallGrab();

        //HandleDashing();
    }

    #region Inputs

    private void GatherInputs()
    {
        inputs.RawX = (int)Input.GetAxisRaw("Horizontal");
        inputs.RawY = (int)Input.GetAxisRaw("Vertical");
        inputs.X = Input.GetAxis("Horizontal");
        inputs.Y = Input.GetAxis("Vertical");
    }

    #endregion

    #region Detection

    private void HandleGrounding()
    {
        // Grounder
        var grounded = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, ground, _groundMask) > 0;

        if (!IsGrounded && grounded)
        {
            IsGrounded = true;
            _hasDashed = false;
            _hasJumped = false;
            currentMovementLerpSpeed = 100;
            PlayRandomClip(_landClips);
            OnTouchedGround?.Invoke();
            transform.SetParent(ground[0].transform);
        }
        else if (IsGrounded && !grounded)
        {
            IsGrounded = false;
            _timeLeftGrounded = Time.time;
            transform.SetParent(null);
        }

        // Wall detection
        _pushingLeftWall = _isAgainstLeftWall && inputs.X < 0;
        _pushingRightWall = _isAgainstRightWall && inputs.X > 0;
    }

    private void DrawGrounderGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, _grounderOffset), _grounderRadius);
    }

    private void OnDrawGizmos()
    {
        DrawGrounderGizmos();
        DrawWallSlideGizmos();
    }

    #endregion

    #region Walking

    [Header("Walking")]
    [SerializeField] private float _walkSpeed = 4;
    [SerializeField] private float _acceleration = 2;
    [SerializeField] private float currentMovementLerpSpeed = 100;

    private void HandleWalking()
    {
        // Slowly release control after wall jump
        currentMovementLerpSpeed = Mathf.MoveTowards(currentMovementLerpSpeed, 100, wallJumpMovementLerp * Time.deltaTime);

        if (dashing) return;
        
        // This can be done using just X & Y input as they lerp to max values, but this gives greater control over velocity acceleration
        var acceleration = IsGrounded ? _acceleration : _acceleration * 0.5f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (rb.velocity.x > 0) inputs.X = 0; // Immediate stop and turn. Just feels better
            inputs.X = Mathf.MoveTowards(inputs.X, -1, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (rb.velocity.x < 0) inputs.X = 0;
            inputs.X = Mathf.MoveTowards(inputs.X, 1, acceleration * Time.deltaTime);
        }
        else
        {
            inputs.X = Mathf.MoveTowards(inputs.X, 0, acceleration * 2 * Time.deltaTime);
        }

        var idealVel = new Vector3(inputs.X * _walkSpeed, rb.velocity.y);
        // _currentMovementLerpSpeed should be set to something crazy high to be effectively instant. But slowed down after a wall jump and slowly released
        rb.velocity = Vector3.MoveTowards(rb.velocity, idealVel, currentMovementLerpSpeed * Time.deltaTime);
    }

    #endregion

    #region Jumping

    [Header("Jumping")][SerializeField] private float _jumpForce = 15;
    [SerializeField] private float _fallMultiplier = 7;
    [SerializeField] private float _jumpVelocityFalloff = 8;
    [SerializeField] private ParticleSystem _jumpParticles;
    [SerializeField] private Transform _jumpLaunchPoof;
    [SerializeField] private float _wallJumpLock = 0.25f;
    [SerializeField] private float wallJumpMovementLerp = 5;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private bool _enableDoubleJump = false;
    private float _timeLeftGrounded = -10;
    private float _timeLastWallJumped;
    private bool _hasJumped;
    private bool _hasDoubleJumped;

    private void HandleJumping()
    {
        if (dashing) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_grabbing || !IsGrounded && (_isAgainstLeftWall || _isAgainstRightWall))
            {
                _timeLastWallJumped = Time.time;
                currentMovementLerpSpeed = wallJumpMovementLerp;
                ExecuteJump(new Vector2(_isAgainstLeftWall ? _jumpForce : -_jumpForce, _jumpForce)); // Wall jump
            }
            else if (IsGrounded || Time.time < _timeLeftGrounded + _coyoteTime || _enableDoubleJump && !_hasDoubleJumped)
            {
                if (!_hasJumped || _hasJumped && !_hasDoubleJumped) ExecuteJump(new Vector2(rb.velocity.x, _jumpForce), _hasJumped); // Ground jump
            }
        }

        void ExecuteJump(Vector3 dir, bool doubleJump = false)
        {
            rb.velocity = dir;
            _jumpLaunchPoof.up = rb.velocity;
            _jumpParticles.Play();
            _hasDoubleJumped = doubleJump;
            _hasJumped = true;
        }

        // Fall faster and allow small jumps. _jumpVelocityFalloff is the point at which we start adding extra gravity. Using 0 causes floating
        if (rb.velocity.y < _jumpVelocityFalloff || rb.velocity.y > 0 && !Input.GetKey(KeyCode.C))
            rb.velocity += _fallMultiplier * Physics.gravity.y * Vector3.up * Time.deltaTime;
    }

    #endregion

    #region Wall Slide

    [Header("Wall Slide")]
    [SerializeField]
    private ParticleSystem _wallSlideParticles;

    [SerializeField] private float _slideSpeed = 1;
    private bool _wallSliding;

    private void HandleWallSlide()
    {
        var sliding = _pushingLeftWall || _pushingRightWall;
        if (sliding && !_wallSliding)
        {
            _wallSliding = true;
            _wallSlideParticles.transform.position = transform.position + new Vector3(_pushingLeftWall ? -_wallCheckOffset : _wallCheckOffset, 0);
            _wallSlideParticles.Play();

            // Don't add sliding until actually falling or it'll prevent jumping against a wall
            if (rb.velocity.y < 0) rb.velocity = new Vector3(0, -_slideSpeed);
        }
        else if (!sliding && _wallSliding && !_grabbing)
        {
            transform.SetParent(null);
            _wallSliding = false;
            _wallSlideParticles.Stop();
        }
    }

    private void DrawWallSlideGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(-_wallCheckOffset, 0), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_wallCheckOffset, 0), _wallCheckRadius);
    }

    #endregion

    #region Wall Grab

    [Header("Wall Grab")][SerializeField] private ParticleSystem _wallGrabParticles;
    private bool _grabbing;

    private void HandleWallGrab()
    {
        // I added wallJumpLock but I honestly can't remember why and I'm too scared to remove it...
        var grabbing = (_isAgainstLeftWall || _isAgainstRightWall) && Input.GetKey(KeyCode.Z) && Time.time > _timeLastWallJumped + _wallJumpLock;

        rb.useGravity = !_grabbing;
        if (grabbing && !_grabbing)
        {
            _grabbing = true;
            _wallGrabParticles.transform.position = transform.position + new Vector3(_pushingLeftWall ? -_wallCheckOffset : _wallCheckOffset, 0);
            _wallGrabParticles.Play();
        }
        else if (!grabbing && _grabbing)
        {
            _grabbing = false;
            _wallGrabParticles.Stop();
            Debug.Log("stopped");
        }

        if (_grabbing) rb.velocity = new Vector3(0, inputs.RawY * _slideSpeed * (inputs.RawY < 0 ? 1 : 0.8f));
    }

    #endregion

    #region Dash

    [Header("Dash")][SerializeField] private float _dashSpeed = 15;
    [SerializeField] private float _dashLength = 1;
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private Transform _dashRing;
    [SerializeField] private ParticleSystem _dashVisual;

    public static event Action OnStartDashing, OnStopDashing;

    private bool _hasDashed;
    private bool dashing;
    private float _timeStartedDash;
    private Vector3 _dashDir;

    private void HandleDashing()
    {
        if (Input.GetKeyDown(KeyCode.X) && !_hasDashed)
        {
            _dashDir = new Vector3(inputs.RawX, inputs.RawY).normalized;
            _dashRing.up = _dashDir;
            _dashParticles.Play();
            dashing = true;
            _hasDashed = true;
            _timeStartedDash = Time.time;
            rb.useGravity = false;
            _dashVisual.Play();
            PlayRandomClip(_dashClips);
            OnStartDashing?.Invoke();
        }

        if (dashing)
        {
            rb.velocity = _dashDir * _dashSpeed;

            if (Time.time >= _timeStartedDash + _dashLength)
            {
                _dashParticles.Stop();
                dashing = false;
                // Clamp the velocity so they don't keep shooting off
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y > 3 ? 3 : rb.velocity.y);
                rb.useGravity = true;
                if (IsGrounded) _hasDashed = false;
                _dashVisual.Stop();
                OnStopDashing?.Invoke();
            }
        }
    }

    #endregion
    #region Impacts

    [Header("Collisions")]
    [SerializeField]
    private ParticleSystem _impactParticles;

    [SerializeField] private GameObject _deathExplosion;
    [SerializeField] private float _minImpactForce = 2;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > _minImpactForce && IsGrounded) _impactParticles.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            Instantiate(_deathExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        _hasDashed = false;
    }

    #endregion

    #region Audio

    [Header("Audio")][SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _landClips;
    [SerializeField] private AudioClip[] _dashClips;

    private void PlayRandomClip(AudioClip[] clips)
    {
        _source.PlayOneShot(clips[Random.Range(0, clips.Length)], 0.2f);
    }

    #endregion

    private struct FrameInputs
    {
        public float X, Y;
        public int RawX, RawY;
    }
}