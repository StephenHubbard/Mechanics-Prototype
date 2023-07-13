using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private GameObject _hitVFX;
    [SerializeField] private float _knockBackForce = 5f;
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private int _damageAmount = 1;

    private bool _isInitialized = false;
    private Vector2 _fireDirection, _previousPosition, _playerPosOnFire;
    private Rigidbody2D _rigidBody;
    private Gun _gun;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _gun = FindFirstObjectByType<Gun>();
    }

    private void Update() {
        CheckCollision();
    }

    private void FixedUpdate()
    {
        if (!_isInitialized) return;

        _rigidBody.velocity = _fireDirection * _moveSpeed;
    }

    public void OnEnable()
    {
        _playerPosOnFire = PlayerController.Instance.transform.position;
        Vector2 bulletSpawnPosition = _gun.BulletSpawnPoint.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _fireDirection = (mousePosition - bulletSpawnPosition).normalized;
        transform.position = bulletSpawnPosition;
        _rigidBody.position = bulletSpawnPosition;
        _previousPosition = _rigidBody.position;
        _isInitialized = true;
    }

    private void OnDisable() {
        _isInitialized = false;
    }

    // continous rb2d detection doesn't work on static tilemap if tilemap also has rb.  Tilemap needs it for composite collider2d for smooth walking for collider movement.
    private void CheckCollision()
    {
        if (!_isInitialized) return;

        Vector2 newPosition = _rigidBody.position;
        Vector2 direction = (newPosition - _previousPosition).normalized;
        float distance = Vector2.Distance(newPosition, _previousPosition);
        
        RaycastHit2D hit = Physics2D.Raycast(_previousPosition, direction, distance, _collisionLayers);

        if (hit.collider != null)
        {
            Collide(hit);
        }
    }

    private void Collide(RaycastHit2D hit)
    {
        Instantiate(_hitVFX, transform.position, Quaternion.identity);

        IDamageable iDamageable = hit.collider.gameObject.GetComponent<IDamageable>();
        iDamageable?.TakeHit(_damageAmount);

        IKnockbackable iKnockbackable = hit.collider.gameObject.GetComponent<IKnockbackable>();
        iKnockbackable?.HandleKnockback(_playerPosOnFire, _knockBackForce);
        
        _isInitialized = false;
        _previousPosition = _rigidBody.position;
        _gun.ReleaseBulletFromPool(this);
    }
}