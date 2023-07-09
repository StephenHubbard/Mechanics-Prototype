using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Grenade : MonoBehaviour
{
    public Action OnBeep;
    public Action OnExplode;

    [SerializeField] private LayerMask _interactLater;
    [SerializeField] private GameObject _explodeVFX;
    [SerializeField] private GameObject _grenadeFlash;
    [SerializeField] private GameObject _grenadeLight;
    [SerializeField] private float _explodeRadius = 3.5f;
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _explodeTime = 3f;
    [SerializeField] private float _knockBackForce = 20f;
    [SerializeField] private int _damageAmount = 3;
    [SerializeField] private int totalBlinks = 7;

    private int currentBlinks = 0;

    private Rigidbody2D _rigidBody;
    private CinemachineImpulseSource _impulseSource;
    private AudioManager _audioManager;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start() {
        LaunchGrenade();
        StartCoroutine(CountdownExplodeRoutine());
    }

    private void OnEnable() {
        OnExplode += DamageNearbyColliders;
        OnExplode += IncreaseScreenShakeByPlayerDistance;
        OnExplode += GrenadeFlash;
        OnExplode += _audioManager.Grenade_OnExplode;
        OnBeep += _audioManager.Grenade_OnBeep;
        OnBeep += BlinkLight;
    }

    private void OnDisable() {
        OnExplode -= DamageNearbyColliders;
        OnExplode -= IncreaseScreenShakeByPlayerDistance;
        OnExplode -= GrenadeFlash;
        OnExplode -= _audioManager.Grenade_OnExplode;
        OnBeep -= _audioManager.Grenade_OnBeep;
        OnBeep -= BlinkLight;
    }

    private void LaunchGrenade()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;
        _rigidBody.AddForce(directionToMouse * _launchForce, ForceMode2D.Impulse);
        float torqueAmount = .2f;
        _rigidBody.AddTorque(torqueAmount, ForceMode2D.Impulse);
    }

    private IEnumerator CountdownExplodeRoutine() {
        while (currentBlinks < totalBlinks)
        {
            yield return new WaitForSeconds(_explodeTime / totalBlinks);
            OnBeep?.Invoke();
            float lightBlinkTime = .15f;
            yield return new WaitForSeconds(lightBlinkTime);
            _grenadeLight.SetActive(false);
        }

        OnExplode?.Invoke();
    }

    private void BlinkLight() {
        _grenadeLight.SetActive(true);
        currentBlinks++;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((_interactLater.value & (1 << other.gameObject.layer)) != 0)
        {
            OnExplode?.Invoke();
        }
    }

    private void DamageNearbyColliders()
    {
        Instantiate(_explodeVFX, transform.position, Quaternion.identity);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explodeRadius, _interactLater);
        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            health?.TakeDamage(_damageAmount);

            if (health && health.CurrentHealth > 0)
            {
                Knockback knockback = hit.GetComponent<Knockback>();
                knockback?.GetKnockedBack(this.transform.position, _knockBackForce);
            }
        }

        Destroy(gameObject);
    }

    private void IncreaseScreenShakeByPlayerDistance()
    {
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);

            float shakeMin = 0.1f;
            float shakeMax = 1f;
            float shakeIntensity = Mathf.Clamp(1f / distanceToPlayer, shakeMin, shakeMax);

            float xMultiplier = 5f;
            float yMultiplier = 3f;
            Vector2 explosionVelocity = new Vector2(shakeIntensity * xMultiplier, shakeIntensity * yMultiplier);

            _impulseSource.GenerateImpulseAt(transform.position, explosionVelocity);
        }
    }

    private void GrenadeFlash() {
        GameObject grenadeFlash = Instantiate(_grenadeFlash, transform.position, Quaternion.identity);
        float destroyTimeDelay = 0.2f;
        Utils.DestroySelf(grenadeFlash, destroyTimeDelay);
    }
}
