using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material _defaultMat;
    [SerializeField] private Material _whiteFlashMat;
    [SerializeField] private float _flashTime = 0.1f;

    private SpriteRenderer[] _spriteRenderers;
    private ColorChanger _colorChanger;

    private void Awake()
    {
        _colorChanger = GetComponent<ColorChanger>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SetDefaultMat();
    }

    public void StartFlash() {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.material = _whiteFlashMat;

            if (_colorChanger) { _colorChanger.SetColor(Color.white); }
        }

        yield return new WaitForSeconds(_flashTime);

        SetDefaultMat();
    }

    private void SetDefaultMat()
    {
        foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.material = _defaultMat;

            if (_colorChanger) { _colorChanger.SetColor(_colorChanger.DefaultColor); }
        }
    }
}
