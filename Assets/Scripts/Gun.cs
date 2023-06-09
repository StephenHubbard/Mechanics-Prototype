using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    public Transform BulletSpawnPoint => _bulletSpawnPoint;
    public bool AttackInput => FrameInput.Attack;
    
    protected FrameInput FrameInput;

    [SerializeField] private float _gunFireCD = .3f;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _rotationClamp = 60f;


    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private ObjectPool<Bullet> _bulletPool;
    private float _lastFireTime = -1; // better to set to -1 than 0 from chatgpt
    private Animator _animator;
    private PlayerInput _playerInput;
    private PlayerController _playerController;
    private CinemachineImpulseSource _fireImpulseSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponentInParent<PlayerInput>();
        _playerController = GetComponentInParent<PlayerController>();
        _fireImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start() {
        CreateBulletPool();
    }

    private void Update() {
        FrameInput = _playerInput.FrameInput;

        if (Time.time >= _lastFireTime + _gunFireCD)
        {
            Shoot();
        }
    }

    // Was getting some jitter since player is moving on FixedUpdate
    private void FixedUpdate() {
        RotateGun();
    }

    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() =>
        {
            return Instantiate(_bulletPrefab);
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet);
        }, false, 50, 100);
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void RotateGun()
    {
        FrameInput = _playerInput.FrameInput;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = _playerController.transform.InverseTransformPoint(mousePosition);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, -_rotationClamp, _rotationClamp);

        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Shoot()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (FrameInput.AttackHeld)
        {
            Bullet newBullet = _bulletPool.Get();
            _animator.Play(FIRE_HASH, 0, 0);
            _lastFireTime = Time.time;

            if (MechanicsManager.Instance.ScreenShakeToggle)
            {
                _fireImpulseSource.GenerateImpulse();
            }
        }
    }

}
