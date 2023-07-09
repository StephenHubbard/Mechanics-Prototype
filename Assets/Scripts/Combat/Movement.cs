using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove;

    [SerializeField] private float _moveSpeed = 3f;

    private float _currentDirection;
    private bool _canMove = true;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetCurrentDirection(float currentDirection)
    {
        _currentDirection = currentDirection;
    }

    public void KnockBackStart()
    {
        _canMove = false;
    }

    public void KnockBackEnd()
    {
        _canMove = true;
    }

    private void Move()
    {
        if (!_canMove) { return; }

        Vector2 newVelocity = new (_currentDirection * _moveSpeed, _rigidBody.velocity.y);
        _rigidBody.velocity = newVelocity;
    }
}