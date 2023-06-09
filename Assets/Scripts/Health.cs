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

    private void Awake() {
        _currentHealth = _startingHealth;
        _knockBack = GetComponent<Knockback>();
    }

    public void TakeDamage() {
        if (!_canTakeDamageCD) { return; }
        
        _currentHealth--;
        _canTakeDamageCD = false;
        StartCoroutine(DetectDeath());
    }

    private IEnumerator DetectDeath() {
        if (!_isPlayer) {
            _canTakeDamageCD = true;
        }

        yield return new WaitForSeconds(_knockBack.KnockBackTime);

        if (_currentHealth <= 0) {
            if (MechanicsManager.Instance.HitFeedbackToggle) {
                Instantiate(_deathVFX, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        } else {
            _canTakeDamageCD = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        EnemyMovement enemy = other.gameObject.GetComponent<EnemyMovement>();

        if (_isPlayer && enemy) {
            TakeDamage();

            if (MechanicsManager.Instance.HitFeedbackToggle)
            {
                PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();

                _knockBack.GetKnockedBack(enemy.transform.position, enemy.KnockbackThrust);
                playerAnimations.ScreenShake();
            }
        }
    }
}
