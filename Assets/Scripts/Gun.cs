using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    protected FrameInput FrameInput;
    public bool AttackInput => FrameInput.Attack;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GameObject _bulletPrefab;

    private PlayerInput _playerInput;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void Update() {
        FrameInput = _playerInput.FrameInput;

        Shoot();
    }

    private void Shoot() {
        if (FrameInput.Attack) {
            GameObject newBullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.transform.position, _playerController.transform.rotation);
        }
    }
}
