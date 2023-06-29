using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int CurrentHealth => _currentHealth;
    public Action OnDeath;
    public static Action<Health> OnEnemyDeath;

    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private AudioClip _onHitSFX;
    [SerializeField] private GameObject _deathSplatter;

    // Don't love doing this but I also don't FindObject of type with an empty class so idk. 
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

        OnDeath += DeathFX;
        OnDeath += DeathSplatter;
    }

    private void OnDisable() {
        OnDeath -= DeathFX;
        OnDeath -= DeathSplatter;
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
        OnDeath?.Invoke();

        bool isEnemy = _enemySpawner;

        if (isEnemy)
        {
            _enemySpawner.ReleaseEnemyFromPool(_enemyMovement);
            _score.EnemyKilled();
            OnEnemyDeath?.Invoke(this);
        }
        else {
            PlayerController.Instance.PlayerDeath();
        }
    }


    private void DeathFX() {
        GameObject deathVFX = Instantiate(_deathVFX, transform.position, Quaternion.identity);
        ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;

        ColorChanger colorChanger = GetComponent<ColorChanger>();

        if (colorChanger)
        {
            ps.startColor = _colorChanger.CurrentColor;
        }
    }

    private void DeathSplatter()
    {
        GameObject newSplatter = Instantiate(_deathSplatter, transform.position, Quaternion.identity);
        SpriteRenderer splatterSpriteRenderer = newSplatter.GetComponent<SpriteRenderer>();
        Transform parentTransform = _splatterParent.transform;
        newSplatter.transform.SetParent(parentTransform);

        ColorChanger colorChanger = GetComponent<ColorChanger>();

        if (colorChanger) {
            splatterSpriteRenderer.color = colorChanger.CurrentColor;
        }
    }
}
