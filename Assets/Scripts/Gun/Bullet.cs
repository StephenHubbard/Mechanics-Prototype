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
    private Vector2 _fireDirection;
    private Vector2 _previousPosition;
    private Vector2 _playerPosOnFire; // getting player pos on fire in case player dies while bullet is in air

    private Rigidbody2D _rb;
    private Gun _gun;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _previousPosition = transform.position;
        _gun = FindObjectOfType<Gun>();
    }

    private void Update() {
        CheckCollision();
    }

    private void FixedUpdate()
    {
        MoveProjectile();
    }

    private void OnEnable()
    {
        _isInitialized = true;
        _playerPosOnFire = PlayerController.Instance.transform.position;
        Vector2 bulletSpawnPosition = _gun.BulletSpawnPoint.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _fireDirection = (mousePosition - bulletSpawnPosition).normalized;
        transform.position = bulletSpawnPosition;
        _previousPosition = transform.position;
    }

    private void OnDisable()
    {
        _isInitialized = false;
    }

    private void MoveProjectile()
    {
        _rb.velocity = _fireDirection * _moveSpeed;
    }

    // continous rb2d detection doesn't work on static tilemap if tilemap also has rb.  Tilemap needs it for composite collider2d.
    private void CheckCollision()
    {
        if (!_isInitialized) return;

        Vector2 newPosition = _rb.position;
        Vector2 direction = (newPosition - _previousPosition).normalized;
        float distance = Vector2.Distance(newPosition, _previousPosition);

        RaycastHit2D hit = Physics2D.Raycast(_previousPosition, direction, distance, _collisionLayers);

        if (hit.collider != null)
        {
            Instantiate(_hitVFX, hit.point, Quaternion.identity);

            Health health = hit.collider.gameObject.GetComponent<Health>();
            health?.TakeDamage(_damageAmount);

            if (health && health.CurrentHealth > 0) {
                Knockback knockback = hit.collider.gameObject.GetComponent<Knockback>();
                knockback?.GetKnockedBack(_playerPosOnFire, _knockBackForce);
            }

            _gun.ReleaseBulletFromPool(this);

            return;
        }

        _previousPosition = newPosition;
    }
}