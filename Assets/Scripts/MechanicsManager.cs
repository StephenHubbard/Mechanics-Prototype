using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MechanicsManager : Singleton<MechanicsManager>
{
    public bool PlayerControllerToggle => _playerControllerToggle.isOn;
    public bool GunToggle => _gunToggle.isOn;
    public bool VfxToggle => _vfxToggle.isOn;
    public bool ScreenShakeToggle => _screenShakeToggle.isOn;
    public bool HitFeedbackToggle => _hitFeedbackToggle.isOn;
    public bool PostProcessingToggle => _postProcessingToggle.isOn;

    [SerializeField] private Toggle _playerControllerToggle;
    [SerializeField] private Toggle _gunToggle;
    [SerializeField] private Toggle _vfxToggle;
    [SerializeField] private Toggle _screenShakeToggle;
    [SerializeField] private Toggle _hitFeedbackToggle;
    [SerializeField] private Toggle _postProcessingToggle;

    private BasicPlayerController _basicPlayerController;
    private PlayerController _playerController;
    private Volume _globalVolume;

    private BasicGun _basicGun;
    private Gun _gun;

    protected override void Awake() {
        base.Awake();

        _basicPlayerController = FindObjectOfType<BasicPlayerController>();
        _playerController = FindObjectOfType<PlayerController>();
        _basicGun = FindObjectOfType<BasicGun>();
        _gun = FindObjectOfType<Gun>();
        _globalVolume = FindObjectOfType<Volume>();
    }

    private void Start() {
        if (_playerControllerToggle.isOn) {
            ImprovedPlayerController();
        }

        if (_gunToggle.isOn) {
            BetterGun();
        }

        if (_postProcessingToggle.isOn)
        {
            PostProccessing();
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

    public void PostProccessing() {
        _globalVolume.enabled = !_globalVolume.enabled;
    }


}
