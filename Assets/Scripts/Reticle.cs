using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private RectTransform _recticle;
    [SerializeField] private float _restingSize = 25.0f;
    private float _currentSize = 0.0f;
    private float _previousSize, _newSize;
    [SerializeField] private float _expandSpeed = 30.0f;
    [SerializeField] private float _multiplyFactor = 700.0f;

    private void Start()
    {
        _recticle = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _currentSize = Mathf.Lerp(_previousSize, _newSize, Time.deltaTime * _expandSpeed);
        _recticle.sizeDelta = new Vector2(_currentSize, _currentSize);
    }

    public void UpdateReticle(float spread)
    {
        _previousSize = _currentSize;
        _newSize = _restingSize + spread * _multiplyFactor;
    }
}
