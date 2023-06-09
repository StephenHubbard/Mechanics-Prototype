using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = false;
    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathVFX;

    private bool _canTakeDamageCD = true;
    private int _currentHealth;
    private Knockback _knockBack;
    private EnemySpawner _enemySpawner;
    private EnemyMovement _enemyMovement;

    private void Awake() {
        _knockBack = GetComponent<Knockback>();
        _enemyMovement = GetComponent<EnemyMovement>();
    }

    private void OnEnable() {
        _currentHealth = _startingHealth;
    }

    public void EnemyInit(EnemySpawner enemySpawner) {
        _enemySpawner = enemySpawner;
    }

    public void TakeDamage(int amount) {
        if (!_canTakeDamageCD) { return; }
        
        _currentHealth -= amount;
        _canTakeDamageCD = false;
        StartCoroutine(DetectDeath());
    }

    private IEnumerator DetectDeath() {
        if (!_isPlayer) {
            _canTakeDamageCD = true;
        }

        if (_isPlayer && _currentHealth > 0) {
            yield return new WaitForSeconds(_knockBack.KnockBackTime);
        } else {
            yield return null;
        }

        if (_currentHealth <= 0) {
            if (MechanicsManager.Instance.HitFeedbackToggle) {
                Instantiate(_deathVFX, transform.position, Quaternion.identity);
            }

            if (MechanicsManager.Instance.ObjectPoolingToggle && _enemySpawner != null) {
                _enemySpawner.ReleaseEnemyFromPool(_enemyMovement);
            } else {
                Destroy(gameObject);
            }
        } else {
            _canTakeDamageCD = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        EnemyMovement enemy = other.gameObject.GetComponent<EnemyMovement>();

        if (_isPlayer && enemy) {
            TakeDamage(1);

            if (MechanicsManager.Instance.HitFeedbackToggle)
            {
                PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();

                _knockBack.GetKnockedBack(enemy.transform.position, enemy.KnockbackThrust);
                playerAnimations.ScreenShake();
            }
        }
    }
}
