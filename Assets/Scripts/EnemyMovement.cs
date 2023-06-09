using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float KnockbackThrust => _knockBackThrust;

    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _changeDirectionTime = 4f;
    [SerializeField] private float _knockBackThrust = 5f;

    private Rigidbody2D _rb;
    private Knockback _knockback;
    private float _currentDirection;
    private Coroutine _changeDirectionCoroutine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        float startMoveDelay = .4f;
        Invoke("StartMoving", startMoveDelay);
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

    public void StartMoving()
    {
        if (_changeDirectionCoroutine != null)
            StopCoroutine(_changeDirectionCoroutine);

        _changeDirectionCoroutine = StartCoroutine(ChangeDirection());
    }

    public void StopMoving()
    {
        if (_changeDirectionCoroutine != null)
            StopCoroutine(_changeDirectionCoroutine);

        _rb.velocity = Vector2.zero;
    }
}
