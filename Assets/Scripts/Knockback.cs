using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool GettingKnockedBack { get; private set; }
    public float KnockBackTime => _knockBackTime;

    [SerializeField] private float _knockBackTime = .2f;
    [SerializeField] private Material _whiteFlashMat;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Rigidbody2D _rb;
    private Material defaultMat;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        defaultMat = _spriteRenderer.material;
    }

    public void GetKnockedBack(Vector2 damageSource, float knockBackThrust)
    {
        if (!MechanicsManager.Instance.HitFeedbackToggle) { return; }

        GettingKnockedBack = true;
        Vector2 difference = ((Vector2)transform.position - damageSource).normalized * knockBackThrust * _rb.mass;
        _rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockBackTime);
        _rb.velocity = Vector2.zero;
        GettingKnockedBack = false;
    }

    private IEnumerator FlashRoutine()
    {
        _spriteRenderer.material = _whiteFlashMat;
        yield return new WaitForSeconds(_knockBackTime / 2);
        _spriteRenderer.material = defaultMat;
    }
}
