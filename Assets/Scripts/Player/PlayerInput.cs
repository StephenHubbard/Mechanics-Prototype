using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }
    private PlayerInputActions _actions;
    private InputAction _move, _attack, _jump, _jetpack, _grenade;

    private void Awake() {
        _actions = new PlayerInputActions();

        _move = _actions.Player.Move;
        _attack = _actions.Player.Attack;
        _jump = _actions.Player.Jump;
        _jetpack = _actions.Player.Jetpack;
        _grenade = _actions.Player.Grenade;
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
            Dash = _jetpack.WasPressedThisFrame(),
            Attack = _attack.WasPressedThisFrame(),
            AttackHeld = _attack.IsPressed(),
            Grenade = _grenade.WasPressedThisFrame(),
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
    public bool Grenade;
    public bool Use;
}
