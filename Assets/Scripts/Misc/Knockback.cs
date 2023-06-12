using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool GettingKnockedBack { get; private set; }
    public float KnockBackTime => _knockBackTime;

    [SerializeField] private float _knockBackTime = .2f;
    
    private Rigidbody2D _rb;
    private ColorChanger _colorChanger;
    private Flash _flash;

    private void Awake()
    {
        _flash = GetComponent<Flash>();
        _colorChanger = GetComponent<ColorChanger>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Vector2 damageSource, float knockBackThrust)
    {
        if (!MechanicsManager.Instance.HitFeedbackToggle) { return; }

        GettingKnockedBack = true;
        Vector2 difference = ((Vector2)transform.position - damageSource).normalized * knockBackThrust * _rb.mass;
        _rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
        StartCoroutine(_flash.FlashRoutine(_knockBackTime));
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(_knockBackTime);
        _rb.velocity = Vector2.zero;
        GettingKnockedBack = false;
    }
}
