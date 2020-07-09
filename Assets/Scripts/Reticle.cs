using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    [Header("Reticle Settings")]
    [SerializeField] private float _restingSize = 25.0f;
    [SerializeField] private float _expandSpeed = 30.0f;
    [SerializeField] private float _multiplyFactor = 700.0f;

    [Header("Hitmarker Settings")]
    [SerializeField] private float _hitmarkerDuration = 0.5f;
    [SerializeField] private float _hitmarkerFadeSpeed = 0.5f;
    [SerializeField] private AudioClip _hitmarkerClip;

    private RectTransform _reticle;
    private float _currentSize = 0.0f;
    private float _previousSize = 0.0f, _newSize = 0.0f;
    private AudioSource _audioSource;
    private Image _hitmarkerImage;
    private float _currentHitmarkerDuration = 0.0f;

    private void Start()
    {
        _reticle = GetComponent<RectTransform>();
        if (_reticle == null) Debug.LogError("Reticle is NULL");
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) Debug.LogError("AudioSource is NULL");
        _hitmarkerImage = GameObject.Find("HitmarkerImage").GetComponent<Image>();
        if (_hitmarkerImage == null) Debug.LogError("Hitmarker Image is NULL");
        _hitmarkerImage.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        _currentSize = Mathf.Lerp(_previousSize, _newSize, Time.deltaTime * _expandSpeed);
        _reticle.sizeDelta = new Vector2(_currentSize, _currentSize);
        if (_currentHitmarkerDuration > 0.0f)
        {
            _currentHitmarkerDuration -= Time.deltaTime;
        }
        else
        {
            _hitmarkerImage.color = Color.Lerp(_hitmarkerImage.color, new Color(1, 1, 1, 0), Time.deltaTime * _hitmarkerFadeSpeed);
        }
    }

    public void UpdateReticle(float spread)
    {
        _previousSize = _currentSize;
        _newSize = _restingSize + spread * _multiplyFactor;
    }

    public void Hitmarker()
    {
        _audioSource.PlayOneShot(_hitmarkerClip);
        _hitmarkerImage.color = new Color(1, 1, 1, 1);
        _currentHitmarkerDuration = _hitmarkerDuration;
    }
}
