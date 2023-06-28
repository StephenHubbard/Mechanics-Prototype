using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material _whiteFlashMat;

    private Material _defaultMat;
    private SpriteRenderer[] _spriteRenderers;
    private ColorChanger _colorChanger;

    private void Awake()
    {
        _colorChanger = GetComponent<ColorChanger>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _defaultMat = _spriteRenderers[0].material;
    }

    private void OnEnable()
    {
        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            sr.material = _defaultMat;
        }
    }

    public IEnumerator FlashRoutine(float knockBackTime)
    {
        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            sr.material = _whiteFlashMat;
            if (_colorChanger) { _colorChanger.SetFillColorWhite(); }
        }

        float flashTime = knockBackTime / 2f;
        yield return new WaitForSeconds(flashTime);

        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            sr.material = _defaultMat;
            if (_colorChanger) { _colorChanger.SetFillColorDefault(); }
        }
    }
}
