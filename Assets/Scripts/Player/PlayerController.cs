using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : Singleton<PlayerController>
{
    public Vector2 MoveInput => FrameInput.Move;

    public static Action OnPlayerHit;
    public static Action OnJetpack;

    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime = .7f;
    [SerializeField] private float _dashCD = 2f;
    [SerializeField] private float _jumpStrength;
    [SerializeField] private float _extraGravity = 50f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private SpriteRenderer _jetPackSpriteRenderer;
    [SerializeField] private GameObject _visuals;

    private FrameInput FrameInput;
    private Quaternion _targetTiltRotation;
    private float _coyoteTimer, _lastDash;
    private Rigidbody2D _rb;
    private TrailRenderer _trailRenderer;
    private PlayerInput _playerInput;
    private Knockback _knockBack;
    private bool _jumping;
    private Fade _fade;
    private Health _health;
    private PlayerAnimations _playerAnimations;
    private Movement _movement;

    protected override void Awake() {
        base.Awake();

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
        _jumping = false;
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
        ExtraGravity();
        Jetpack();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (!enemy) { return; }

        OnPlayerHit?.Invoke();
        int enemyDamageAmount = 1;
        _health.TakeDamage(enemyDamageAmount);
        _knockBack.GetKnockedBack(enemy.transform.position, enemy.KnockbackThrust);
        _playerAnimations.ScreenShake();
    }

    public bool IsFacingRight()
    {
        return transform.eulerAngles.y == 0;
    }

    public void PlayerDeath() {
        _fade.FadeIn();
        Destroy(gameObject);
    }

    private void GatherInput()
    {
        FrameInput = _playerInput.FrameInput;
        _movement.SetCurrentDirection(FrameInput.Move.x);
    }

    private void CoyoteTimer()
    {
        _coyoteTimer = CheckGrounded() ? _coyoteTime : _coyoteTimer - Time.deltaTime;
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
        if (FrameInput.Jump && !_jumping && _coyoteTimer > 0 && CheckGrounded())
        {
            _rb.velocity = Vector2.up * _jumpStrength;
            _coyoteTimer = 0;
            _jumping = true;
            StartCoroutine(JumpCDRoutine());
        }
    }

    private void ExtraGravity() {
        if (!CheckGrounded())
        {
            _rb.AddForce(new Vector2(0, -_extraGravity * Time.deltaTime));
        }
    }

    private IEnumerator JumpCDRoutine()
    {
        float jumpRefreshTime = .2f;
        yield return new WaitForSeconds(jumpRefreshTime);
        _jumping = false;
    }

    private void Jetpack()
    {
        if (FrameInput.Dash && Time.time >= _lastDash + _dashCD)
        {
            OnJetpack?.Invoke();
            _trailRenderer.enabled = true;
            Vector2 direction = _rb.velocity.normalized;
            _rb.velocity = direction * _dashSpeed;
            _lastDash = Time.time;
            StartCoroutine(JetpackRoutine());
        }
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
