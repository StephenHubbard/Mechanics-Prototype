using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathVFX;

    private int _currentHealth;

    private void Awake() {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage() {
        _currentHealth--;
        DetectDeath();
    }

    private void DetectDeath() {
        if (_currentHealth <= 0) {
            if (MechanicsManager.Instance.HitFeedbackToggle) {
                Instantiate(_deathVFX, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
