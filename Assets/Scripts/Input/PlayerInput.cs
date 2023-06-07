using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }

    private PlayerInputActions _actions;
    private InputAction _move, _attack, _jump, _dash;

    private void Awake() {
        _actions = new PlayerInputActions();

        _move = _actions.Player.Move;
        _attack = _actions.Player.Attack;
        _jump = _actions.Player.Jump;
        _dash = _actions.Player.Dash;
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
            Jump = _jump.WasPressedThisFrame(),
            Dash = _dash.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            Attack = _attack.WasPressedThisFrame(),
            AttackHeld = _attack.IsPressed(),
        };
    }
}

public struct FrameInput
{
    public Vector2 Move;
    public bool Attack;
    public bool AttackHeld;
    public bool Dash;
    public bool Jump;
    public bool JumpHeld;
}
