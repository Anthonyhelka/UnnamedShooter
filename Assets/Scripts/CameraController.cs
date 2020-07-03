using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _player;
    private Transform _playerHead;
    private float _mouseX, _mouseY, _xRotation;
    [SerializeField] private float _mouseSensitivity = 50.0f;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Transform>();
        if (_player == null) Debug.LogError("Player is NULL");
        _playerHead = GameObject.Find("PlayerHead").GetComponent<Transform>();
        if (_playerHead == null) Debug.LogError("PlayerHead is NULL");
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        GetInput();

        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -60.0f, 60.0f);
        transform.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);
        _playerHead.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);

        _player.Rotate(0.0f, _mouseX * _mouseSensitivity * Time.deltaTime, 0.0f);
    }

    void GetInput()
    {
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");
    }
}
