using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knockback : MonoBehaviour
{
    public Action OnKnockbackStart;
    public Action OnKnockbackEnd;
    
    public float KnockBackTime => _knockBackTime;

    [SerializeField] private float _knockBackTime = .2f;

    private Vector3 _hitDirection;
    private float _knockBackThrust;

    private Rigidbody2D _rigidBody;

    private void OnEnable() {
        OnKnockbackStart += ApplyKnockbackForce;
        OnKnockbackEnd += StopKnockbackForce;
    }

    private void OnDisable() {
        
        OnKnockbackStart -= ApplyKnockbackForce;
        OnKnockbackEnd -= StopKnockbackForce;
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Vector3 hitDirection, float knockbackThrust)
    {
        _hitDirection = hitDirection;
        _knockBackThrust = knockbackThrust;

        OnKnockbackStart?.Invoke();
    }

    private void ApplyKnockbackForce()
    {
        Vector3 difference = (transform.position - _hitDirection).normalized * _knockBackThrust * _rigidBody.mass;
        _rigidBody.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockBackTime);
        OnKnockbackEnd?.Invoke();
    }

    private void StopKnockbackForce()
    {
        _rigidBody.velocity = Vector2.zero;
    }
}