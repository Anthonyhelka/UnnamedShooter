using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float _range = 100.0f;
    [SerializeField] private float _minRecoil = 0.4f;
    [SerializeField] private float _maxRecoil = 0.6f;
    [SerializeField] private float _impactForce = 100.0f;
    [SerializeField] private float _visualRecoilAngle = 2.5f;
    [SerializeField] private float _sprintSlerpSpeed = 5.0f;

    [Header("Primary Fire Settings")]
    [SerializeField] private float _primaryDamage = 10.0f;
    [SerializeField] private float _primaryFireRate = 0.3f;

    [Header("Secondary Fire Settings")]
    [SerializeField] private float _secondaryDamage = 8.0f;
    [SerializeField] private float _secondaryFireRate = 0.75f;
    [SerializeField] private float _burstFireRate = 0.15f;
    [SerializeField] private float _burstFireAmount = 3;

    private Camera _playerCamera;
    private ParticleSystem _muzzleFlash;
    private Transform _cameraContainer;
    private Animator _animator;
    private AudioSource _audioSource;

    private bool _canFire = true;
    private float _fireCooldown = -0.1f;
    private float _recoil = 0.0f;
    private Vector3 _sprintSmoothDampVelocity, _recoilSmoothDampVelocity =  Vector3.zero;
    private float _totalVisualRecoilAngle;
    private float _recoilAngleSmoothDampVelocity = 0.0f;
    private Quaternion _targetSprintRotation = Quaternion.Euler(new Vector3(2.1f, -53.2f, 34.0f));
    private float _totalSprintSlerp;

    void  Start()
    {
        _playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        if (_playerCamera == null) Debug.LogError("PlayerCamera is NULL");
        _muzzleFlash = GameObject.Find("MuzzleFlash").GetComponent<ParticleSystem>();
        if (_muzzleFlash == null) Debug.LogError("MuzzleFlash is NULL");
        _cameraContainer = GameObject.Find("CameraContainer").GetComponent<Transform>();
        if (_cameraContainer == null) Debug.LogError("CameraContainer is NULL");
        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogError("Animator is NULL");
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) Debug.LogError("AudioSource is NULL");
    }

    void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0.125f, -0.225f, 0.45f), ref _recoilSmoothDampVelocity, 0.1f);
        _totalVisualRecoilAngle = Mathf.SmoothDamp(_totalVisualRecoilAngle, 0, ref _recoilAngleSmoothDampVelocity, 0.1f);
        transform.localEulerAngles = transform.localEulerAngles = Vector3.left * _totalVisualRecoilAngle;
    }

    public void PrimaryFire()
    {
        if (_fireCooldown < Time.time && _canFire)
        {
            FireCoroutine(_primaryDamage, _primaryFireRate);
            _fireCooldown = Time.time + _primaryFireRate;
        }
    }

    public void SecondaryFire()
    {
        if (_fireCooldown < Time.time && _canFire)
        {
            StartCoroutine(BurstCoroutine());
            _fireCooldown = Time.time + _secondaryFireRate;
        }
    }

    private void FireCoroutine(float damage, float fireRate)
    {
        _muzzleFlash.Play();
        _audioSource.Play();
        Recoil();
        RaycastHit hit;
        if(Physics.Raycast(_playerCamera.transform.position,_playerCamera.transform.forward, out hit, _range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * _impactForce);
            }
        }
    }

    private void Recoil()
    {
        _recoil += Random.Range(_minRecoil, _maxRecoil);
        _cameraContainer.transform.localRotation = Quaternion.Euler(-_recoil, 0.0f, 0.0f);
        transform.localPosition -= Vector3.forward * 0.1f;
        _totalVisualRecoilAngle += _visualRecoilAngle;
        _totalVisualRecoilAngle = Mathf.Clamp(_totalVisualRecoilAngle, 0, 30);
    }

    private IEnumerator BurstCoroutine()
    {   
        for (int i = 0; i < _burstFireAmount; i++) 
            {
                FireCoroutine(_secondaryDamage, _burstFireRate);
                yield return new WaitForSeconds(_burstFireRate);
            }
    }

    public void Sprint(bool isSprinting)
    {   
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0.07f, -0.27f, 0.44f), ref _sprintSmoothDampVelocity, 2.0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetSprintRotation, _totalSprintSlerp);
        if (isSprinting)
        {   
            _totalSprintSlerp += _sprintSlerpSpeed * Time.deltaTime;
        }
        else if(!isSprinting)    
        {   
            _totalSprintSlerp -= _sprintSlerpSpeed * Time.deltaTime;
        }
        _totalSprintSlerp = Mathf.Clamp(_totalSprintSlerp, 0, 1);
        _canFire = _totalSprintSlerp == 0 ? true : false;
    }
}
