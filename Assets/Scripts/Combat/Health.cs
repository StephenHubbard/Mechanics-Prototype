 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int CurrentHealth => _currentHealth;
    public GameObject DeathSplatter => _deathSplatter;
    public GameObject DeathVFX => _deathVFX;

    public static Action<Health> OnDeath;

    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathSplatter;
    [SerializeField] private GameObject _deathVFX;

    private int _currentHealth;
    private Knockback _knockBack;
    private Pipe _enemySpawner;
    private Enemy _enemy;
    private ColorChanger _colorChanger;
    private Score _score;
    private DeathSplatterHandler _splatterParent;

    private void Awake() {
        _knockBack = GetComponent<Knockback>();
        _enemy = GetComponent<Enemy>();
        _colorChanger = GetComponent<ColorChanger>();
        _score = FindObjectOfType<Score>();
        _splatterParent = FindObjectOfType<DeathSplatterHandler>();
    }

    private void Start() {
        _currentHealth = _startingHealth;
    }
    
    public void EnemyInit(Pipe enemySpawner) {
        _currentHealth = _startingHealth;
        _enemySpawner = enemySpawner;
    }

    public void TakeDamage(int amount) {
        _currentHealth -= amount;

        if (_currentHealth <= 0) { DetectDeath(); }
    }

    private void DetectDeath() {
        OnDeath?.Invoke(this);

        if (_enemy)
        {
            _enemySpawner.ReleaseEnemyFromPool(_enemy);
        }
        else {
            PlayerController.Instance.PlayerDeath();
        }
    }
}
