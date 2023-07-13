using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallexOffset = -0.1f;

    private Camera _mainCamera;
    private Vector2 _startPos;
    private Vector2 _travel => (Vector2)_mainCamera.transform.position - _startPos;

    private void Awake() {
        _mainCamera = Camera.main;
    }

    private void Start() {
        _startPos = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = _startPos + new Vector2(_travel.x * parallexOffset, 0);
        transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);
    }

}
