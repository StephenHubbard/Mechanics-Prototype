using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerAnimations : MonoBehaviour
{
    private FrameInput FrameInput;

    public Vector2 MoveInput => FrameInput.Move;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private SpriteRenderer _playerSpriteRenderer;
    [SerializeField] private float _tiltAngle = 10f;    
    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private float _yLandImpactDustEffect = -12f;
    [SerializeField] private ParticleSystem _landDustVFX;
    [SerializeField] private ParticleSystem _moveDustVFX;

    private Vector2 _moveDir;
    private Vector3 _velocityBeforePhysicsUpdate;

    private PlayerInput _playerInput;
    private Rigidbody2D _rb;
    private PlayerController _playerController;
    private CinemachineImpulseSource _impulseSource;

    public void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update() {
        GatherInput();
        ApplyTilt();
        DetectMoveEffect();
    }

    private void FixedUpdate() {
        _velocityBeforePhysicsUpdate = _rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & _groundLayer.value) != 0 && _velocityBeforePhysicsUpdate.y <= _yLandImpactDustEffect)
        {
            _landDustVFX.Play();
            ScreenShake();
        }
    }

    public void ScreenShake() {
        _impulseSource.GenerateImpulse();
    }

    public void MoveDustOff() {
        if (_moveDustVFX.isPlaying)
        {
            _moveDustVFX.Stop();
        }
    }

    private void DetectMoveEffect() {
        if (_playerController.CheckGrounded())
        {
            if (!_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Play();
            }
        }
        else
        {
            if (_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Stop();
            }
        }
    }

    private void GatherInput()
    {
        FrameInput = _playerInput.FrameInput;
        _moveDir.x = FrameInput.Move.x;
    }

    private void ApplyTilt()
    {
        float targetAngle = MoveInput.x < 0 ? _tiltAngle : MoveInput.x > 0 ? -_tiltAngle : 0f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        Quaternion currentRotation = _playerSpriteRenderer.transform.rotation;

        targetRotation.eulerAngles = new Vector3(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, targetRotation.eulerAngles.z);

        _playerSpriteRenderer.transform.rotation = Quaternion.Lerp(_playerSpriteRenderer.transform.rotation, targetRotation, _tiltSpeed * Time.deltaTime);
    }
}
