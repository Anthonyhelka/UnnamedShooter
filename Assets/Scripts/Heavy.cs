using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float _range = 100.0f;
    [SerializeField] private float _impactForce = 100.0f;
    [SerializeField] private int _magazineSize = 20;
    [SerializeField] private int _magazine = 20;
    [SerializeField] private float _reloadDuration = 1.0f;
    [SerializeField] private float _criticalMultiplier = 2.0f;
    [SerializeField] private float _visualRecoilKickback = 0.03f;
    [SerializeField] private float _visualRecoilAngle = 1.0f;
    [SerializeField] private float _sprintSlerpPositionSpeed = 6.3f;
    [SerializeField] private float _sprintSlerpRotationSpeed = 5.0f;
    [SerializeField] private float _minSpread = 0.0f;
    [SerializeField] private float _maxSpread = 0.05f;
    [SerializeField] private float _spreadIncrease = 0.01f;
    [SerializeField] private float _spreadRecoveryTime = 0.3f;
    [SerializeField] private float _spreadRecoverySpeed = 30.0f;
    
    [Header("Primary Fire Settings")]
    [SerializeField] private float _primaryDamage = 10.0f;
    [SerializeField] private float _primaryFireRate = 0.3f;

    [Header("Secondary Fire Settings")]
    [SerializeField] private float _secondaryDamage = 6.0f;
    [SerializeField] private float _secondaryFireRate = 0.3f;
    [SerializeField] private float _burstFireRate = 0.15f;
    [SerializeField] private float _burstFireAmount = 3;

    [Header("Assets")]
    [SerializeField] private AudioClip _primaryFireAudioClip;
    [SerializeField] private AudioClip _secondaryFireAudioClip;
    [SerializeField] private GameObject impactEffect;

    [SerializeField] private UI _uiScript;
    private ParticleSystem _muzzleFlash;
    private Transform _cameraContainer;
    private Animator _animator;
    private AudioSource _audioSource;

    private bool _canFire = true;
    private float _fireCooldown = -0.1f;
    private Vector3 _sprintSmoothDampVelocity, _recoilSmoothDampVelocity = Vector3.zero;
    private float _totalVisualRecoilAngle;
    private float _recoilAngleSmoothDampVelocity = 0.0f;
    private Vector3 _targetSprintPosition = new Vector3(-0.1f, -0.26f, 0.4f);
    private Quaternion _targetSprintRotation = Quaternion.Euler(new Vector3(10.0f, -55.0f, 40.0f));
    private float _totalSprintPositionSlerp, _totalSprintRotationSlerp;
    private Transform _shootPoint;
    private Vector3 _shootDirection;
    private float _totalSpread;
    private float _spreadRecovery;
    private Reticle _reticle;

    private void Start()
    {
        _shootPoint = GameObject.Find("PlayerCamera").GetComponent<Transform>();
        if (_shootPoint == null) Debug.LogError("PlayerCamera is NULL");
        _muzzleFlash = GameObject.Find("MuzzleFlash").GetComponent<ParticleSystem>();
        if (_muzzleFlash == null) Debug.LogError("MuzzleFlash is NULL");
        _cameraContainer = GameObject.Find("CameraContainer").GetComponent<Transform>();
        if (_cameraContainer == null) Debug.LogError("CameraContainer is NULL");
        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogError("Animator is NULL");
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) Debug.LogError("AudioSource is NULL");
        _reticle = GameObject.Find("Reticle").GetComponent<Reticle>();
        if (_reticle == null) Debug.LogError("Reticle is NULL");
        _uiScript = GameObject.Find("Canvas").GetComponent<UI>();
        if (_uiScript == null) Debug.LogError("UIScript is NULL");
        _uiScript.UpdateMagazine(_magazine, _magazineSize);
    }

    private void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0.045f, -0.0625f, 0.15f), ref _recoilSmoothDampVelocity, 0.1f);
        _totalVisualRecoilAngle = Mathf.SmoothDamp(_totalVisualRecoilAngle, 0, ref _recoilAngleSmoothDampVelocity, 0.1f);
        // transform.localEulerAngles = transform.localEulerAngles = Vector3.left * _totalVisualRecoilAngle;
        if (_spreadRecovery < Time.time)
        {
            _totalSpread -= _spreadIncrease * _spreadRecoverySpeed * Time.deltaTime;
            _totalSpread = Mathf.Clamp(_totalSpread, _minSpread, _maxSpread);
        }
        _reticle.UpdateReticle(_totalSpread);
    }

    public void PrimaryFire()
    {
        if (_fireCooldown < Time.time && _canFire && _magazine > 0)
        {
            _fireCooldown = Time.time + _primaryFireRate;
            Fire(_primaryDamage, _primaryFireRate);
        }
    }

    public void SecondaryFire()
    {
        if (_fireCooldown < Time.time && _canFire && _magazine > 0)
        {
            _fireCooldown = Time.time + _secondaryFireRate;
            StartCoroutine(BurstCoroutine());
        }
    }

    private void Fire(float damage, float fireRate, bool primaryFire = true)
    {
        _muzzleFlash.Play();
        if (primaryFire) {
            _audioSource.PlayOneShot(_primaryFireAudioClip);
        } else {
            _audioSource.PlayOneShot(_secondaryFireAudioClip);
        }
        Recoil();
        CalculateSpread();
        RaycastHit hit;
        _magazine--;
        _uiScript.UpdateMagazine(_magazine, _magazineSize);
        if (_magazine <= 0) {
            StartCoroutine(Reload());
        }
        if(Physics.Raycast(_shootPoint.position, _shootDirection, out hit, _range))
        {
            Debug.Log(hit.transform.tag);
            Target target = hit.transform.parent.GetComponent<Target>();
            if (target != null)
            {
                if (hit.transform.tag == "Critical") 
                {
                    target.TakeDamage(damage * _criticalMultiplier);
                    _reticle.Hitmarker(true);
                } 
                else
                {
                    target.TakeDamage(damage);
                    _reticle.Hitmarker(false);
                }
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * _impactForce);
            }

            GameObject impactGameObject = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGameObject, 10.0f);
        }
    }

    private void Recoil()
    {
        transform.localPosition -= Vector3.forward * _visualRecoilKickback;
        _totalVisualRecoilAngle += _visualRecoilAngle;
        _totalVisualRecoilAngle = Mathf.Clamp(_totalVisualRecoilAngle, 0, 30);
    }

    private void CalculateSpread()
    {
        _totalSpread += _spreadIncrease;
        _totalSpread = Mathf.Clamp(_totalSpread, _minSpread, _maxSpread);
        _shootDirection = _shootPoint.transform.forward;
        _shootDirection.x += Random.Range(-_totalSpread, _totalSpread);
        _shootDirection.y += Random.Range(-_totalSpread, _totalSpread);
        _spreadRecovery = _spreadRecoveryTime + Time.time;
    }

    private IEnumerator BurstCoroutine()
    {   
        for (int i = 0; i < _burstFireAmount; i++) 
            {
                Fire(_secondaryDamage, _burstFireRate, false);
                _fireCooldown += _burstFireRate;
                yield return new WaitForSeconds(_burstFireRate);
            }
    }

    private IEnumerator Reload()
    {   
        _canFire = false;
        _uiScript.UpdateMagazine(_magazine, _magazineSize, true);
        float duration = 0.0f;
        while (duration < _reloadDuration) {
            duration += Time.deltaTime;
            yield return 0;
        }
        _canFire = true;
        _magazine = _magazineSize;
        _uiScript.UpdateMagazine(_magazine, _magazineSize);
    }

    public void Sprint(bool isSprinting)
    {   
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetSprintPosition, _totalSprintPositionSlerp);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetSprintRotation, _totalSprintRotationSlerp);
        if (isSprinting)
        {   
            _totalSprintPositionSlerp += _sprintSlerpPositionSpeed * Time.deltaTime;
            _totalSprintRotationSlerp += _sprintSlerpRotationSpeed * Time.deltaTime;
        }
        else if(!isSprinting)    
        {   
            _totalSprintPositionSlerp -= _sprintSlerpPositionSpeed * Time.deltaTime;
            _totalSprintRotationSlerp -= _sprintSlerpRotationSpeed * Time.deltaTime;
        }
        _totalSprintPositionSlerp = Mathf.Clamp(_totalSprintPositionSlerp, 0, 1);
        _totalSprintRotationSlerp = Mathf.Clamp(_totalSprintRotationSlerp, 0, 1);
        _canFire = _totalSprintRotationSlerp == 0 && _totalSprintPositionSlerp == 0 ? true : false;
    }
}
