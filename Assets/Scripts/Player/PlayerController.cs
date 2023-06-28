using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    protected FrameInput FrameInput;

    public Vector2 MoveInput => FrameInput.Move;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime = .7f;
    [SerializeField] private float _dashCD = 2f;
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _maxFallSpeed = -10f;
    [SerializeField] private float _extraGravity = 50f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private SpriteRenderer _jetPackSpriteRenderer;
    [SerializeField] private GameObject _visuals;

    private Quaternion _targetTiltRotation;
    private Vector2 _moveDir;
    private float _coyoteTimer, _lastDash;
    private Rigidbody2D _rb;
    private TrailRenderer _trailRenderer;
    private PlayerInput _playerInput;
    private Knockback _knockBack;
    private bool _jumping, _dashing;
    private Sounds _sound;

    protected override void Awake() {
        base.Awake();

        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _knockBack = GetComponent<Knockback>();
        _sound = GetComponent<Sounds>();
    }

    private void Start() {
        _jumping = false;
        _dashing = false;
        _lastDash = -_dashCD;
    }

    private void OnEnable() {
        _jetPackSpriteRenderer.enabled = true;
    }

    private void OnDisable() {
        _jetPackSpriteRenderer.enabled = false;
    }

    private void Update()
    {
        GatherInput();
        HandleSpriteFlip();
        CoyoteTimer();
        Jump();
        Dash();
    }

    private void FixedUpdate() {
        Movement();
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    }

    private void GatherInput()
    {
        FrameInput = _playerInput.FrameInput;
        _moveDir.x = FrameInput.Move.x;
    }

    private void Movement() {
        if (_dashing || _knockBack.GettingKnockedBack) { return; }

        Vector2 newVelocity = new Vector2(_moveDir.x * _moveSpeed, _rb.velocity.y);
        _rb.velocity = newVelocity;

        if (_rb.velocity.y < _maxFallSpeed)
        {
            // come back to
            Debug.Log("max fall speed");
            _rb.velocity = new Vector2(_rb.velocity.x, _maxFallSpeed);
        }
    }

    private void CoyoteTimer()
    {
        if (CheckGrounded())
        {
            _coyoteTimer = _coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    public Collider2D CheckGrounded() {
        Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0, _groundLayer);

        return isGrounded;
    }

    // CheckGrounded() Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }

    private void Jump()
    {
        if (!CheckGrounded())
        {
            _rb.AddForce(new Vector2(0, -_extraGravity * Time.deltaTime));
        }

        if (FrameInput.Jump && !_jumping && _coyoteTimer > 0 && CheckGrounded())
        {
            _rb.velocity = Vector2.up * _jumpStrength;
            _coyoteTimer = 0;
            _jumping = true;
            StartCoroutine(JumpCDRoutine());
        }
    }

    private IEnumerator JumpCDRoutine()
    {
        float jumpRefreshTime = .2f;
        yield return new WaitForSeconds(jumpRefreshTime);
        _jumping = false;
    }

    private void Dash()
    {
        if (FrameInput.Dash && Time.time >= _lastDash + _dashCD)
        {
            _dashing = true;
            _trailRenderer.enabled = true;
            _sound.PlaySound(0);
            Vector2 direction = _rb.velocity.normalized;

            _rb.velocity = direction * _dashSpeed;

            _lastDash = Time.time;
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        yield return new WaitForSeconds(_dashTime);
        _dashing = false;
        yield return new WaitForSeconds(.2f);
        _trailRenderer.enabled = false;
    }

    private void HandleSpriteFlip()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    } 
    
}
