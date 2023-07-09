using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IBulletCollideable
{
    public float KnockbackThrust => _knockBackThrust;
    public int EnemyDamageAmount => _damageAmount;

    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _changeDirectionTime = 4f;
    [SerializeField] private float _knockBackThrust = 5f;
    [SerializeField] private float _jumpForce = 5f; 
    [SerializeField] private float _jumpInterval = 5f; 
    [SerializeField] private float _startMoveDelay = 0.4f;

    private Pipe _pipe;
    private Rigidbody2D _rigidBody;
    private Knockback _knockback;
    private Movement _movement;
    private Health _health;
    private ColorChanger _colorChanger;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
        _movement = GetComponent<Movement>();
        _health = GetComponent<Health>();
        _colorChanger = GetComponent<ColorChanger>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_startMoveDelay);
        StartCoroutine(ChangeDirection());
        StartCoroutine(RandomJump());
    }

    public void EnemyInit(Pipe pipe, Color color)
    {
        _health.ResetHealth();
        _pipe = pipe;
        transform.position = pipe.transform.position;
        _colorChanger.SetColor(color);
    }

    public void EnemyDeath()
    {
        _pipe.ReleaseEnemyFromPool(this);
    }

    public void TakeHit(RaycastHit2D hit, Vector2 playerPosOnFire, float knockbackForce, int damageAmount) {
        if (_health && _health.CurrentHealth > 0)
        {
            Knockback knockback = hit.collider.gameObject.GetComponent<Knockback>();
            knockback?.GetKnockedBack(playerPosOnFire, knockbackForce);

            Health health = hit.collider.gameObject.GetComponent<Health>();
            health?.TakeDamage(damageAmount);
        }
    }

    private IEnumerator ChangeDirection()
    {
        while (true)
        {
            float currentDirection = Mathf.Sign(Random.Range(-1, 1));
            _movement.SetCurrentDirection(currentDirection);
            yield return new WaitForSeconds(_changeDirectionTime);
        }
    }

    private IEnumerator RandomJump() 
    {
        while (true)
        {
            yield return new WaitForSeconds(_jumpInterval);

            if (_movement.CanMove) { 
                float randomDirection = Random.Range(-1, 1);
                Vector2 jumpDirection = new Vector2(randomDirection, 1).normalized;

                _rigidBody.AddForce(jumpDirection * _jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}
