using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knockback : MonoBehaviour
{
    public float KnockBackTime => _knockBackTime;

    public event Action OnKnockBackStart;
    public event Action OnKnockBackEnd;

    [SerializeField] private float _knockBackTime = .2f;

    private Rigidbody2D _rb;
    private ColorChanger _colorChanger;
    private Flash _flash;
    

    private void Awake()
    {
        _flash = GetComponent<Flash>();
        _colorChanger = GetComponent<ColorChanger>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Vector2 damageSource, float knockBackThrust)
    {
        Vector2 difference = ((Vector2)transform.position - damageSource).normalized * knockBackThrust * _rb.mass;
        _rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
        StartCoroutine(_flash.FlashRoutine());

        OnKnockBackStart?.Invoke();
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockBackTime);
        _rb.velocity = Vector2.zero;

        OnKnockBackEnd?.Invoke();
    }
}