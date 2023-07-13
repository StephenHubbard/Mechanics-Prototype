 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int CurrentHealth { get; private set; }
    public GameObject DeathSplatter => _deathSplatter;
    public GameObject DeathVFX => _deathVFX;

    public static Action<Health> OnDeath;

    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathSplatter;
    [SerializeField] private GameObject _deathVFX;

    private Score _score;

    private void Awake() {
        _score = FindFirstObjectByType<Score>();
    }

    private void Start() {
        ResetHealth();
    }

    public bool IsEnemy() {
        Enemy enemy = GetComponent<Enemy>();
        return enemy;
    }

    public void ResetHealth() {
        CurrentHealth = _startingHealth;
    }

    public void TakeDamage(int amount) {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0) { DetectDeath(); }
    }

    private void DetectDeath()
    {
        OnDeath?.Invoke(this);

        if (TryGetComponent(out Enemy enemy))
        {
            enemy.EnemyDeath();
        }
        else
        {
            PlayerController.Instance.PlayerDeath();
        }
    }
}
