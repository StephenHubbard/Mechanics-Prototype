using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Vector2 MoveInput => FrameInput.Move;
    public static Action OnPlayerHit;
    public static Action OnJetpack;
    public static Action OnJump;

    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime = .7f;
    [SerializeField] private float _dashCD = 2f;
    [SerializeField] private float _lastJumpBuffer = .3f;
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _extraGravity = 50f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private GameObject _visuals;

    private Quaternion _targetTiltRotation;
    private float _coyoteTimer, _lastDash, _lastJumpPressed;
    private bool _doubleJumpAvailable, _coyoteJump;

    private FrameInput FrameInput;
    private Rigidbody2D _rb;
    private TrailRenderer _trailRenderer;
    private PlayerInput _playerInput;
    private Knockback _knockBack;
    private Fade _fade;
    private Health _health;
    private PlayerAnimations _playerAnimations;
    private Movement _movement;

    public void Awake() {
        if (Instance == null) { Instance = this; }

        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _knockBack = GetComponent<Knockback>();
        _fade = FindObjectOfType<Fade>();
        _health = GetComponent<Health>();
        _playerAnimations = GetComponent<PlayerAnimations>();
        _movement = GetComponent<Movement>();
    }

    private void Start() {
        _lastJumpPressed = -_lastJumpBuffer;
        _lastDash = -_dashCD;
    }

    private void OnEnable() {
        OnPlayerHit += PlayerHit;
        OnJetpack += Dash;
        OnJump += ApplyJumpForce;
    }

    private void OnDisable() {
        OnPlayerHit -= PlayerHit;
        OnJetpack -= Dash;
        OnJump -= ApplyJumpForce;
    }

    private void Update()
    {
        GatherInput();
        HandleSpriteFlip();
        CoyoteTimer();
        HandleJump();
        ExtraGravity();
        Jetpack();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy && _movement.CanMove) { 
            OnPlayerHit?.Invoke();
            _knockBack.GetKnockedBack(enemy.transform.position, enemy.KnockbackThrust);
        }
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    }

    public void PlayerDeath() {
        _fade.FadeIn();
        Destroy(gameObject);
    }

    private void PlayerHit() {
        int enemyDamageAmount = 1;
        _health.TakeDamage(enemyDamageAmount);
        _playerAnimations.ScreenShake();
    }

    private void GatherInput()
    {
        FrameInput = _playerInput.FrameInput;
        _movement.SetCurrentDirection(FrameInput.Move.x);
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

    private void HandleJump()
    {
        if (!FrameInput.Jump) return;

        if (CheckGrounded())
        {
            Jump();
        }
        else if (_coyoteTimer > 0)
        {
            Jump();
        }
        else if (_doubleJumpAvailable)
        {
            _doubleJumpAvailable = false;
            Jump();
        }
    }

    private void Jump()
    {
        OnJump?.Invoke();
    }

    private void ApplyJumpForce() {
        _rb.velocity = Vector2.zero;
        _rb.velocity = Vector2.up * _jumpStrength;
        _lastJumpPressed = Time.time;
        _coyoteTimer = 0;
        _playerAnimations.PlayDustVFX();
    }

    private void CoyoteTimer()
    {
        if (CheckGrounded())
        {
            _doubleJumpAvailable = true;
            _coyoteTimer = _coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    private void ExtraGravity()
    {
        if (!CheckGrounded())
        {
            _rb.AddForce(new Vector2(0, -_extraGravity * Time.deltaTime));
        }
    }


    private void Jetpack()
    {
        if (FrameInput.Dash && Time.time >= _lastDash + _dashCD)
        {
            OnJetpack?.Invoke();
        }
    }

    private void Dash() {
        _trailRenderer.enabled = true;
        Vector2 direction = _rb.velocity.normalized;
        _rb.velocity = direction * _dashSpeed;
        _lastDash = Time.time;
        StartCoroutine(JetpackRoutine());
    }

    private IEnumerator JetpackRoutine()
    {
        yield return new WaitForSeconds(_dashTime);
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
