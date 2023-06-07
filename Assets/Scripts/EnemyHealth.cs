using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int _currentHealth = 3;

    private void Start() {
        
    }

    public void TakeDamage() {
        _currentHealth--;
        DetectDeth();
    }

    private void DetectDeth() {
        if (_currentHealth <= 0) {
            Destroy(gameObject);
        }
    }
}
