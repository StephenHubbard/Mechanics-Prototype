using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private GameObject _hitVFX;
    [SerializeField] private float _knockBackForce = 5f;
    [SerializeField] private LayerMask _collisionLayers;

    private bool _isInitialized = false;

    private Rigidbody2D _rb;
    private Vector2 _previousPosition;
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
        transform.SetPositionAndRotation(_gun.BulletSpawnPoint.position, _gun.transform.rotation);
        _previousPosition = transform.position;
        _isInitialized = true;
    }

    private void OnDisable() {
        _isInitialized = false;
    }

    private void MoveProjectile()
    {
        Vector2 direction = transform.right;
        _rb.velocity = direction * _moveSpeed;
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
            if (MechanicsManager.Instance.VFXToggle)
            {
                Instantiate(_hitVFX, hit.point, Quaternion.identity);
            }

            Health health = hit.collider.gameObject.GetComponent<Health>();
            health?.TakeDamage();

            Knockback knockback = hit.collider.gameObject.GetComponent<Knockback>();
            knockback?.GetKnockedBack(PlayerController.Instance.transform.position, _knockBackForce);

            _previousPosition = _rb.position;
            _gun.ReleaseBulletFromPool(this);
            return;
        }

        _previousPosition = newPosition;
    }
}