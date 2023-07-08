using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Pool;
using System;

public class Gun : MonoBehaviour
{
    public Transform BulletSpawnPoint => _bulletSpawnPoint;
    public bool AttackInput => FrameInput.Attack;

    public static Action OnShoot;
    public static Action OnGrenadeShoot;

    [SerializeField] private float _gunFireCD = .3f;
    [SerializeField] private float _grenadeThrowCD = 1f;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Grenade _grenadePrefab;
    [SerializeField] private float _rotationClamp = 60f;
    [SerializeField] private float muzzleFlashTime = .07f;

    private Coroutine muzzleFlashRoutine;
    private FrameInput FrameInput;
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private ObjectPool<Bullet> _bulletPool;
    private float _lastFireTime = -1; // better to initialize it to -1 than 0 (from chatgpt)
    private float _lastThrowTime = -1; 
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

    private void OnEnable() {
        OnShoot += Shoot;
        OnShoot += MuzzleFlash;
        OnGrenadeShoot += ThrowGrenade;
    }

    private void OnDisable() {
        OnShoot -= Shoot;
        OnShoot -= MuzzleFlash;
        OnGrenadeShoot -= ThrowGrenade;
    }

    private void Update() {
        if (Utils.IsOverUI()) { return; }

        FrameInput = _playerInput.FrameInput;

        if (FrameInput.AttackHeld && Time.time >= _lastFireTime + _gunFireCD)
        {
            OnShoot?.Invoke();
        }

        if (FrameInput.Grenade && Time.time >= _lastThrowTime + _grenadeThrowCD)
        {
            OnGrenadeShoot?.Invoke();
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
        Bullet newBullet = _bulletPool.Get();
        _animator.Play(FIRE_HASH, 0, 0);
        _lastFireTime = Time.time;
        _fireImpulseSource.GenerateImpulse();
    }

    private void MuzzleFlash() {
        if (muzzleFlashRoutine != null) { StopCoroutine(muzzleFlashRoutine); }

        muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine() {
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(muzzleFlashTime);
        _muzzleFlash.SetActive(false);
    }

    private void ThrowGrenade() {
        Grenade newGrenade = Instantiate(_grenadePrefab, _bulletSpawnPoint.position, Quaternion.identity);
        _lastThrowTime = Time.time;
    }

}
