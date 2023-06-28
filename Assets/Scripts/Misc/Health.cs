using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int CurrentHealth => _currentHealth;

    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private AudioClip _onHitSFX;
    [SerializeField] private AudioClip _onDeathSFX;
    [SerializeField] private GameObject _deathSplatter;

    const string SPLATTER_PARENT = "Splatter Parent";

    private int _currentHealth;
    private Knockback _knockBack;
    private Pipe _enemySpawner;
    private EnemyMovement _enemyMovement;
    private ColorChanger _colorChanger;
    private Transform _splatterParent;
    private Score _score;

    private void Awake() {
        _knockBack = GetComponent<Knockback>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _colorChanger = GetComponent<ColorChanger>();
        _splatterParent = GameObject.Find(SPLATTER_PARENT).transform;
        _score = FindObjectOfType<Score>();
    }

    private void OnEnable() {
        _currentHealth = _startingHealth;
    }

    public void EnemyInit(Pipe enemySpawner) {
        _enemySpawner = enemySpawner;
    }

    public void TakeDamage(int amount) {
        if (_onHitSFX) { AudioManager.Instance.PlaySound(_onHitSFX, .7f, 1f); }
        
        _currentHealth -= amount;

        if (_currentHealth <= 0) { DetectDeath(); }
    }

    private void DetectDeath() {
        if (_onDeathSFX) { AudioManager.Instance.PlaySound(_onDeathSFX, .8f, 1f); }

        GameObject deathVFX = Instantiate(_deathVFX, transform.position, Quaternion.identity);
        GameObject newSplatter = DeathSplatter();

        bool isEnemy = _enemySpawner;

        if (isEnemy)
        {
            EnemyDeath(deathVFX, newSplatter);
        }
        else {
            PlayerController.Instance.PlayerDeath();
            Destroy(gameObject);
        }
    }

    private void EnemyDeath(GameObject deathVFX, GameObject newSplatter)
    {
        ColorChanger colorChanger = GetComponent<ColorChanger>();
        SpriteRenderer splatterSpriteRenderer = newSplatter.GetComponent<SpriteRenderer>();
        splatterSpriteRenderer.color = colorChanger.CurrentColor;
        ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;
        ps.startColor = _colorChanger.CurrentColor;
        _enemySpawner.ReleaseEnemyFromPool(_enemyMovement);
        _score.EnemyKilled();
    }

    private GameObject DeathSplatter()
    {
        GameObject newSplatter = Instantiate(_deathSplatter, transform.position, Quaternion.identity);
        Transform parentTransform = _splatterParent.transform;
        newSplatter.transform.SetParent(parentTransform);
        return newSplatter;
    }
}
