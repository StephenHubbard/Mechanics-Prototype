using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Health : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = false;
    [SerializeField] private int _startingHealth = 3;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private AudioClip _onHitSFX;
    [SerializeField] private AudioClip _onDeathSFX;
    [SerializeField] private AudioMixerGroup deathSplatterMixerGroup;
    [SerializeField] private GameObject _deathSplatter;

    private bool _canTakeDamageCD = true;
    private int _currentHealth;
    private Knockback _knockBack;
    private Pipe _enemySpawner;
    private EnemyMovement _enemyMovement;
    private Sounds _sounds; 
    private ColorChanger _colorChanger;
    private SplatterParent _splatterParent;
    private Fade _fade;
    private Score _score;

    private void Awake() {
        _sounds = GetComponent<Sounds>();
        _knockBack = GetComponent<Knockback>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _colorChanger = GetComponent<ColorChanger>();
        _splatterParent = FindObjectOfType<SplatterParent>();
        _fade = FindObjectOfType<Fade>();
        _score = FindObjectOfType<Score>();
    }

    private void OnEnable() {
        _currentHealth = _startingHealth;
    }

    public void EnemyInit(Pipe enemySpawner) {
        _enemySpawner = enemySpawner;
    }

    public void TakeDamage(int amount) {
        if (!_canTakeDamageCD) { return; }

        if (_onHitSFX) { AudioManager.Instance.PlaySound(_onHitSFX, null, .7f, 1f); }
        
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
            if (_onDeathSFX) { AudioManager.Instance.PlaySound(_onDeathSFX, deathSplatterMixerGroup, .8f, 1f); }

            GameObject deathVFX = Instantiate(_deathVFX, transform.position, Quaternion.identity);
            
            if (_colorChanger) {
                ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;
                ps.startColor = _colorChanger.CurrentColor;
            }

            GameObject newSplatter = Instantiate(_deathSplatter, transform.position, Quaternion.identity);
            Transform parentTransform = _splatterParent.transform;
            newSplatter.transform.SetParent(parentTransform);
            SpriteRenderer splatterSpriteRenderer = newSplatter.GetComponent<SpriteRenderer>();
            ColorChanger colorChanger = GetComponent<ColorChanger>();

            if (colorChanger) {
                splatterSpriteRenderer.color = colorChanger.CurrentColor;
            }

            if (_isPlayer) {
                _fade.RespawnPlayer();
            }

            if (_enemySpawner != null) {
                _enemySpawner.ReleaseEnemyFromPool(_enemyMovement);
                _score.EnemyKilled();
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

            PlayerAnimations playerAnimations = GetComponent<PlayerAnimations>();

            _knockBack.GetKnockedBack(enemy.transform.position, enemy.KnockbackThrust);
            playerAnimations.ScreenShake();
        }
    }
}
