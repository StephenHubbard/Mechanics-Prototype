using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Vector2 MoveInput => _frameInput.Move;
    
    public static Action<Enemy> OnPlayerHit;
    public static Action OnJetpack;
    public static Action OnJump;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private GameObject _visuals;
    [SerializeField] private float _jetPackStrength = 1f;
    [SerializeField] private float _jetPackTime = .7f;
    [SerializeField] private float _jetPackCD = 2f;
    [SerializeField] private float _lastJumpBuffer = .3f;
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _extraGravity = 50f;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _gravityDelay = .4f;

    private float _coyoteTimer, _lastDash, _lastJumpPressed, _timeInAir;
    private bool _doubleJumpAvailable, _coyoteJump;

    private PlayerInput _playerInput;
    private FrameInput _frameInput;
    private Rigidbody2D _rigidBody;
    private TrailRenderer _trailRenderer;
    private Knockback _knockBack;
    private Fade _fade;
    private Health _health;
    private PlayerAnimations _playerAnimations;
    private Movement _movement;

    public void Awake() {
        if (Instance == null) { Instance = this; }

        _rigidBody = GetComponent<Rigidbody2D>();
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
        _lastDash = -_jetPackCD;
    }

    private void OnEnable() {
        OnPlayerHit += PlayerHit;
        OnJetpack += JetpackDash;
        OnJump += ApplyJumpForce;
        OnJump += ResetGravityDelay;

    }

    private void OnDisable() {
        OnPlayerHit -= PlayerHit;
        OnJetpack -= JetpackDash;
        OnJump -= ApplyJumpForce;
        OnJump -= ResetGravityDelay;
    }

    private void Update()
    {
        GatherInput();
        Movement();
        HandleSpriteFlip();
        CoyoteTimer();
        HandleJump();
        GravityDelay();
        Jetpack();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy && _movement.CanMove) {
            OnPlayerHit?.Invoke(enemy);
        }
    }

    // CheckGrounded() Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position, _groundCheck);
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    }

    public void PlayerDeath() {
        _fade.FadeIn();
        Destroy(gameObject);
    }

    public Collider2D CheckGrounded() {
        Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position, _groundCheck, 0, _groundLayer);
        return isGrounded;
    }

    private void GatherInput()
    {
        _frameInput = _playerInput.FrameInput;
    }

    private void Movement() {
        _movement.SetCurrentDirection(_frameInput.Move.x);
    }

    private void PlayerHit(Enemy enemy)
    {
        int damgeAmount = enemy.EnemyDamageAmount;
        _health.TakeDamage(damgeAmount);
        _playerAnimations.ScreenShake();
        Vector3 hitDirection = enemy.transform.position;
        float knockBackThrust = enemy.KnockbackThrust;
        _knockBack.GetKnockedBack(hitDirection, knockBackThrust);
    }

    private void HandleJump()
    {
        if (!_frameInput.Jump) return;

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
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        _lastJumpPressed = Time.time;
        _coyoteTimer = 0;
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

    private void ResetGravityDelay() {
        _timeInAir = 0f;
    }

    private void GravityDelay()
    {
        if (!CheckGrounded())
        {
            _timeInAir += Time.deltaTime;
        }
        else
        {
            ResetGravityDelay();
        }

        if (_timeInAir > _gravityDelay)
        {
            ExtraGravity();
        }
    }

    private void ExtraGravity()
    {
        if (!CheckGrounded())
        {
            _rigidBody.AddForce(new Vector2(0, -_extraGravity * Time.deltaTime));
        }
    }

    private void Jetpack()
    {
        if (_frameInput.Dash && Time.time >= _lastDash + _jetPackCD)
        {
            OnJetpack?.Invoke();
        }
    }

    private void JetpackDash() {
        _trailRenderer.enabled = true;
        _lastDash = Time.time;
        StartCoroutine(JetpackRoutine());
    }

    private IEnumerator JetpackRoutine()
    {
        float jetTime = 0f;

        while (jetTime < _jetPackTime)
        {
            jetTime += Time.deltaTime;
            _rigidBody.velocity = Vector2.up * _jetPackStrength;
            yield return null;
        }

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
