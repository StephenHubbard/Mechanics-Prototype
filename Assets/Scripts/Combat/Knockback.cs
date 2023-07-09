using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knockback : MonoBehaviour
{
    public float KnockBackTime => _knockBackTime;

    [SerializeField] private float _knockBackTime = .2f;

    private Rigidbody2D _rigidBody;
    private ColorChanger _colorChanger;
    private Flash _flash;
    private Movement _movement;

    private void Awake()
    {
        _flash = GetComponent<Flash>();
        _colorChanger = GetComponent<ColorChanger>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _movement = GetComponent<Movement>();
    }

    public void GetKnockedBack(Vector3 hitDirection, float knockbackThrust)
    {
        Vector3 difference = (transform.position - hitDirection).normalized * knockbackThrust * _rigidBody.mass;
        _rigidBody.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
        StartCoroutine(_flash.FlashRoutine());
        _movement.KnockBackStart();
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockBackTime);
        _rigidBody.velocity = Vector2.zero;
        _movement.KnockBackEnd();
    }
}