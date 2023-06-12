using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Color CurrentColor { get; private set; }

    [SerializeField] private bool _setColorOnStart = false;
    [SerializeField] private Color _pinkColor;
    [SerializeField] private Color _greenColor;
    [SerializeField] private Color _blueColor;
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
        int randomNum = Random.Range(0, 3);

        switch (randomNum)
        {
            case 0:
                CurrentColor = _pinkColor;
                break;

            case 1:
                CurrentColor = _greenColor;
                break;

            case 2:
                CurrentColor = _blueColor;
                break;
            
            default:
                Debug.Log("Color does not exist");
                break;
        }

        _fillSpriteRenderer.color = CurrentColor;
    }
}
