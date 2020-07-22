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
    [SerializeField] private float _hitmarkerDuration = 0.05f;
    [SerializeField] private float _hitmarkerFadeSpeed = 15.0f;
    [SerializeField] private AudioClip _hitmarkerClip;

    private RectTransform _reticle;
    private float _currentSize = 0.0f;
    private float _previousSize = 0.0f, _newSize = 0.0f;
    private AudioSource _audioSource;
    private Image _hitmarkerImage;
    private float _currentHitmarkerDuration = 0.0f;
    private GameObject _hitmarker;
    private Image[] _hitmarkerLines;
    private Color _normalHit = new Color(1.0f, 1.0f, 1.0f, 0.9f);
    private  Color _criticalHit =  new Color(0.94f, 0.51f, 0.47f, 0.9f);

    private void Start()
    {
        _reticle = GetComponent<RectTransform>();
        if (_reticle == null) Debug.LogError("Reticle is NULL");
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) Debug.LogError("AudioSource is NULL");
        _hitmarker = GameObject.Find("Hitmarker");
        if (_hitmarker == null) Debug.LogError("Hitmarker is NULL");
        _hitmarkerLines = _hitmarker.GetComponentsInChildren<Image>();
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
            foreach(Image line in _hitmarker.GetComponentsInChildren<Image>()) 
            {
                Color tempColor = line.color;
                tempColor.a = 0.0f;
                line.color = Color.Lerp(line.color, tempColor, Time.deltaTime * _hitmarkerFadeSpeed);
            }
        }
    }

    public void UpdateReticle(float spread)
    {
        _previousSize = _currentSize;
        _newSize = _restingSize + spread * _multiplyFactor;
    }

    public void Hitmarker(bool critical)
    {
        _audioSource.PlayOneShot(_hitmarkerClip);
        _currentHitmarkerDuration = _hitmarkerDuration;
        foreach(Image line in _hitmarker.GetComponentsInChildren<Image>()) 
        {
            if (critical)
            {
                line.color = _criticalHit;
            }
            else
            {
                line.color = _normalHit;
            }
        }
    }
}
