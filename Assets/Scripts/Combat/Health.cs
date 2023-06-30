 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int CurrentHealth => _currentHealth;

    public static Action<Health> OnDeath;

    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private AudioClip _onHitSFX;
    [SerializeField] private GameObject _deathSplatter;

    private int _currentHealth;
    private Knockback _knockBack;
    private Pipe _enemySpawner;
    private Enemy _enemyMovement;
    private ColorChanger _colorChanger;
    private Score _score;
    private SplatterParent _splatterParent;

    private void Awake() {
        _knockBack = GetComponent<Knockback>();
        _enemyMovement = GetComponent<Enemy>();
        _colorChanger = GetComponent<ColorChanger>();
        _score = FindObjectOfType<Score>();
        _splatterParent = FindObjectOfType<SplatterParent>();
    }

    private void OnEnable()
    {
        OnDeath += DeathFX;
        OnDeath += HandleDeathSplatter;
    }

    private void OnDisable()
    {
        OnDeath -= DeathFX;
        OnDeath -= HandleDeathSplatter;
    }

    private void HandleDeathSplatter(Health sender) {
        if (sender != this) { return; }

        _splatterParent.DeathSplatter(transform, _deathSplatter, _colorChanger);
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

        bool isEnemy = _enemySpawner;

        if (isEnemy)
        {
            _enemySpawner.ReleaseEnemyFromPool(_enemyMovement);
            _score.EnemyKilled();
        }
        else {
            PlayerController.Instance.PlayerDeath();
        }
    }

    private void DeathFX(Health sender) {
        if (sender != this) { return; }

        GameObject deathVFX = Instantiate(_deathVFX, transform.position, Quaternion.identity);
        ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;

        ColorChanger colorChanger = GetComponent<ColorChanger>();

        if (colorChanger) { ps.startColor = _colorChanger.CurrentColor; }
    }
}
