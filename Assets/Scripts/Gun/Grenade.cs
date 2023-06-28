using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private LayerMask _interactLater;
    [SerializeField] private GameObject _explodeVFX;
    [SerializeField] private float _explodeRadius = 3.5f;
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _explodeTime = 3f;
    [SerializeField] private GameObject _grenadeLight;
    [SerializeField] private float _knockBackForce = 20f;


    private Rigidbody2D _rb;
    private CinemachineImpulseSource _impulseSource;
    private Sounds _sound;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _sound = GetComponent<Sounds>();
    }

    private void Start() {
        LaunchGrenade();
        StartCoroutine(CountdownExplodeRoutine());
    }

    private IEnumerator CountdownExplodeRoutine() {
        int totalBlinks = 3;
        int currentBlinks = 0;

        while (currentBlinks < totalBlinks)
        {
            yield return new WaitForSeconds(_explodeTime / totalBlinks);
            _grenadeLight.SetActive(true);
            _sound.PlaySound(0);
            currentBlinks++;
            yield return new WaitForSeconds(.1f);
            _grenadeLight.SetActive(false);
        }

        Explode();
    }

    private void LaunchGrenade() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

        _rb.AddForce(directionToMouse * _launchForce, ForceMode2D.Impulse);

        float torqueAmount = .2f; 
        _rb.AddTorque(torqueAmount, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((_interactLater.value & (1 << other.gameObject.layer)) != 0)
        {
            Explode();
        }
    }

    private void Explode() {
        Instantiate(_explodeVFX, transform.position, Quaternion.identity);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explodeRadius, _interactLater);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            health?.TakeDamage(3);

            Knockback knockback = hit.GetComponent<Knockback>();
            knockback?.GetKnockedBack(this.transform.position, _knockBackForce);
        }

        if (PlayerController.Instance != null) {
            float distanceToPlayer = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
            float shakeIntensity = Mathf.Clamp(1f / distanceToPlayer, 0.05f, 1f);
            Vector3 explosionVelocity = new Vector3(5f * shakeIntensity, 3f * shakeIntensity, 0f);
            _impulseSource.GenerateImpulseAt(transform.position, explosionVelocity);
        } 

        Destroy(gameObject);
    }
}
