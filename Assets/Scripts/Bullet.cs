using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private GameObject _hitVFX;
    [SerializeField] private float _knockBackForce = 5f;

    private void Update() {
        MoveProjectile();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MechanicsManager.Instance.VfxToggle)
        {
            Instantiate(_hitVFX, transform.position, Quaternion.identity);
        }

        Health health = other.gameObject.GetComponent<Health>();
        health?.TakeDamage();

        Knockback knockback = other.gameObject.GetComponent<Knockback>();
        knockback?.GetKnockedBack(PlayerController.Instance.transform.position, _knockBackForce);

        Destroy(gameObject);
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * (_moveSpeed * Time.deltaTime));
    }
}
