using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float KnockbackThrust => _knockBackThrust;

    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _changeDirectionTime = 4f;
    [SerializeField] private float _knockBackThrust = 5f;
    [SerializeField] private float _jumpForce = 5f; 
    [SerializeField] private float _jumpInterval = 5f; 
    [SerializeField] private float _startMoveDelay = 0.4f;

    private Rigidbody2D _rb;
    private Knockback _knockback;
    private float _currentDirection;
    private Pipe _enemySpawner;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_startMoveDelay);
        StartMoving();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_knockback.GettingKnockedBack) { return; }

        _rb.velocity = new Vector2(_currentDirection * _moveSpeed, _rb.velocity.y);
    }

    private IEnumerator ChangeDirection()
    {
        while (true)
        {
            _currentDirection = Mathf.Sign(Random.Range(-1, 1));
            yield return new WaitForSeconds(_changeDirectionTime);
        }
    }

    private IEnumerator RandomJump() 
    {
        while (true)
        {
            yield return new WaitForSeconds(_jumpInterval);

            if (_knockback.GettingKnockedBack) { continue; }

            float randomDirection = Mathf.Sign(Random.Range(-1, 1));
            Vector2 jumpDirection = new Vector2(randomDirection, 1).normalized;

            _rb.AddForce(jumpDirection * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void StartMoving()
    {
        if (!gameObject.activeInHierarchy) { return; }

        StartCoroutine(ChangeDirection());
        StartCoroutine(RandomJump()); 
    }
}
