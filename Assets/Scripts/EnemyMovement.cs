using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _changeDirectionTime = 4f;

    private Rigidbody2D _rb;
    private float _currentDirection;
    private Coroutine _changeDirectionCoroutine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
