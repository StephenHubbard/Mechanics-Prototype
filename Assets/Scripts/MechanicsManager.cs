using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechanicsManager : Singleton<MechanicsManager>
{
    public bool PlayerControllerToggle => _playerControllerToggle.isOn;
    public bool GunToggle => _gunToggle.isOn;
    public bool VfxToggle => _vfxToggle.isOn;
    public bool ScreenShakeToggle => _screenShakeToggle.isOn;

    [SerializeField] private Toggle _playerControllerToggle;
    [SerializeField] private Toggle _gunToggle;
    [SerializeField] private Toggle _vfxToggle;
    [SerializeField] private Toggle _screenShakeToggle;

    private BasicPlayerController _basicPlayerController;
    private PlayerController _playerController;

    private BasicGun _basicGun;
    private Gun _gun;

    protected override void Awake() {
        base.Awake();

        _basicPlayerController = FindObjectOfType<BasicPlayerController>();
        _playerController = FindObjectOfType<PlayerController>();
        _basicGun = FindObjectOfType<BasicGun>();
        _gun = FindObjectOfType<Gun>();
    }

    private void Start() {
        if (_playerControllerToggle.isOn) {
            ImprovedPlayerController();
        }

        if (_gunToggle.isOn) {
            BetterGun();
        }
    }

    public void ImprovedPlayerController() {
        _basicPlayerController.enabled = !_basicPlayerController.enabled;
        _playerController.enabled = !_playerController.enabled;
    }

    public void BetterGun() {
        _basicGun.enabled = !_basicGun.enabled;
        _gun.enabled = !_gun.enabled;
    }


}
