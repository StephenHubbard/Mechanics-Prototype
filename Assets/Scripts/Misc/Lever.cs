using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    private bool _isActive = false;

    private PlayerInputActions _actions;
    private SpriteRenderer _spriteRenderer;

    private void Awake() {
        _actions = new PlayerInputActions();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        _actions.Enable();
    }

    private void OnDisable() {
        _actions.Disable();
    }

    private void Start() {
        _actions.Player.Use.performed += _ => DetectFlip();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<PlayerController>()) {

            _isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            _isActive = false;
        }
    }

    private void DetectFlip() {
        if (!_isActive) { return; }

        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
}
