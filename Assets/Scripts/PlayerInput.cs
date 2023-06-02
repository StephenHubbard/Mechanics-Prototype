using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private PlayerInputActions _actions;
    private InputAction _move;

    private void Awake() {
        _actions = new PlayerInputActions();
        _move = _actions.Player.Move;
    }

    private void Update() {
        FrameInput = GatherInput();
    }

    private void OnEnable() => _actions.Enable();

    private void OnDisable() => _actions.Disable();

    private FrameInput GatherInput()
    {
        return new FrameInput {
            Move = _move.ReadValue<Vector2>(),
        };
    }
}

public struct FrameInput
{
    public Vector2 Move;
}
