using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected FrameInput FrameInput;

    public Vector2 Input => FrameInput.Move;

    [SerializeField] private float _moveSpeed;

    private Vector2 _moveDir;

    private Rigidbody2D _rb;
    private PlayerInput _playerInput;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start() {
        
    }

    private void Update()
    {
        GatherInput();
    }

    private void FixedUpdate() {
        // Movement();
    }
    
    private void GatherInput()
    {
        FrameInput = _playerInput.FrameInput;
        _moveDir.x = FrameInput.Move.x;
    }

    private void Movement() {
        _rb.velocity = _moveDir * (_moveSpeed * Time.fixedDeltaTime);
    }
}
