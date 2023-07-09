using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Color CurrentColor { get; private set; }

    [SerializeField] private bool _setColorOnStart = false;
    [SerializeField] private Color[] _colors;
    [SerializeField] private SpriteRenderer _fillSpriteRenderer;

    private SpriteRenderer[] _spriteRenderers;
    
    private void Awake() {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start() {
        if (_setColorOnStart) {
            SetRandomColor();
        }
    }

    public void SetFillColorDefault() {
        _fillSpriteRenderer.color = CurrentColor;
    }

    public void SetFillColorWhite()
    {
        _fillSpriteRenderer.color = Color.white;
    }

    public void SetColor(Color color) {
        CurrentColor = color;
        _fillSpriteRenderer.color = CurrentColor;
    }

    public void SetRandomColor() {
        int randomNum = Random.Range(0, _colors.Length);
        CurrentColor = _colors[randomNum];
        _fillSpriteRenderer.color = CurrentColor;
    }
}
