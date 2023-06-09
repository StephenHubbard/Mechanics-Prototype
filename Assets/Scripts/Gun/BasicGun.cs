using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class BasicGun : MonoBehaviour
{
    protected FrameInput FrameInput;

    public bool AttackInput => FrameInput.Attack;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GameObject _bulletPrefab;

    private PlayerInput _playerInput;
    private PlayerController _playerController;
    private CinemachineImpulseSource _fireImpulseSource;


    private void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _playerController = GetComponentInParent<PlayerController>();
        _fireImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable() {
        if (_playerController.IsFacingRight()) {
            transform.rotation = Quaternion.identity;
        } else {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
        }
    }

    private void Update() {
        FrameInput = _playerInput.FrameInput;

        Shoot();
    }

    private void Shoot() {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (FrameInput.Attack) {
            GameObject newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.transform.position, _playerController.transform.rotation);

            if (MechanicsManager.Instance.ScreenShakeToggle)
            {
                _fireImpulseSource.GenerateImpulse();
            }
        }
    }
}
