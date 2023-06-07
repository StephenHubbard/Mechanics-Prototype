using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private GameObject _hitVFX;

    private void Update() {
        MoveProjectile();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (MechanicsManager.Instance.VfxToggle) {
            Instantiate(_hitVFX, transform.position, Quaternion.identity);
        }

        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        enemyHealth?.TakeDamage();
        
        Destroy(gameObject);
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * (_moveSpeed * Time.deltaTime));
    }
}
