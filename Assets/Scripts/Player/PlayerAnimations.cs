using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _playerCharacterTransform;
    [SerializeField] private Transform _playerHatTransform;
    [SerializeField] private float _tiltAngle = 10f;    
    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private float _yLandImpactDustEffect = -12f;
    [SerializeField] private ParticleSystem _dustVFX;
    [SerializeField] private ParticleSystem _moveDustVFX;

    private Vector2 _moveDir;
    private Vector3 _velocityBeforePhysicsUpdate;

    private PlayerInput _playerInput;
    private Rigidbody2D _rigidBody;
    private PlayerController _playerController;
    private CinemachineImpulseSource _impulseSource;

    public void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable() {
        PlayerController.OnJump += PlayDustVFX;
    }

    private void OnDisable() {
        PlayerController.OnJump -= PlayDustVFX;
    }

    private void Update() {
        ApplyTilt();
        DetectMoveEffect();
    }

    private void FixedUpdate() {
        _velocityBeforePhysicsUpdate = _rigidBody.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & _groundLayer.value) != 0 && _velocityBeforePhysicsUpdate.y <= _yLandImpactDustEffect)
        {
            PlayDustVFX();
            ScreenShake();
        }
    }

    public void PlayDustVFX() {
        _dustVFX.Play();
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

    private void ApplyTilt()
    {
        float targetAngle;

        if (_playerController.MoveInput.x < 0)
        {
            targetAngle = _tiltAngle;
        }
        else if (_playerController.MoveInput.x > 0)
        {
            targetAngle = -_tiltAngle;
        }
        else
        {
            targetAngle = 0f;
        }
        
        Quaternion currentRotationChar = _playerCharacterTransform.rotation;
        Quaternion targetRotationChar = Quaternion.Euler(currentRotationChar.eulerAngles.x, currentRotationChar.eulerAngles.y, targetAngle);
        _playerCharacterTransform.rotation = Quaternion.Lerp(currentRotationChar, targetRotationChar, _tiltSpeed * Time.deltaTime);

        // would make a nice challenge to mimic above
        float tiltHatModifier = 4f;
        Quaternion currentRotationHat = _playerHatTransform.rotation;
        Quaternion targetRotationHat = Quaternion.Euler(currentRotationHat.eulerAngles.x, currentRotationHat.eulerAngles.y, -targetAngle);
        _playerHatTransform.rotation = Quaternion.Lerp(currentRotationHat, targetRotationHat, _tiltSpeed * tiltHatModifier * Time.deltaTime);
    }
}
