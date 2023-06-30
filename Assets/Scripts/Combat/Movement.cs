using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove;

    [SerializeField] private float _moveSpeed = 3f;

    private float _currentDirection;
    private bool _canMove = true;

    private Rigidbody2D _rb;
    private Knockback _knockback;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    private void OnEnable() {
        _knockback.OnKnockBackStart += KnockBackStart;
        _knockback.OnKnockBackEnd += KnockBackEnd;
    }

    private void OnDisable() {
        _knockback.OnKnockBackStart -= KnockBackStart;
        _knockback.OnKnockBackEnd -= KnockBackEnd;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetCurrentDirection(float currentDirection)
    {
        _currentDirection = currentDirection;
    }

    private void KnockBackStart()
    {
        _canMove = false;
    }

    private void KnockBackEnd()
    {
        _canMove = true;
    }

    private void Move()
    {
        if (!_canMove) { return; }

        Vector2 newVelocity = new Vector2(_currentDirection * _moveSpeed, _rb.velocity.y);
        _rb.velocity = newVelocity;
    }
}