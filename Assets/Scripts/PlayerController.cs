using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float runningSpeed = 11.5f;
    [SerializeField] private float _jumpHeight = 8.0f;
    [SerializeField] private float _gravity = 20.0f;
    private CharacterController _characterController;
    private float _horizontalInput, _verticalInput;
    private bool _sprintInput, _jumpInput, _primaryFireInput, _secondaryFireInput;
    private Vector3 _direction;
    private Heavy _heavy;
    private Animator _playerAnimator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null) Debug.LogError("CharacterController is NULL");
        _playerAnimator = GameObject.Find("CameraContainer").GetComponent<Animator>();
        if (_playerAnimator == null) Debug.LogError("PlayerAnimator is NULL");
        _heavy = GameObject.Find("VP95").GetComponent<Heavy>();
        if (_heavy == null) Debug.LogError("Heavy is NULL");
    }

    private void Update()
    {
        GetInput();
        CalculateMovement();
        FireWeapon();
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _sprintInput = Input.GetKey(KeyCode.LeftShift);
        _jumpInput = Input.GetKeyDown(KeyCode.Space);
        _primaryFireInput = Input.GetButton("Fire1");
        _secondaryFireInput = Input.GetButton("Fire2");
    }

    private void CalculateMovement()
    {
        float directionY = _direction.y;
        _direction = new Vector3(_horizontalInput, 0.0f, _verticalInput).normalized;

        if (_sprintInput && _verticalInput > 0.0f)
        {
            _direction.x = runningSpeed * _direction.x;
            _direction.z = runningSpeed * _direction.z;
            _heavy.Sprint(true);
            if (_characterController.isGrounded)
            {
                _playerAnimator.SetBool("isRunning", true);
            }
            else
            {
                _playerAnimator.SetBool("isRunning", false);
            }
        }
        else
        {
            _direction.x = walkingSpeed * _direction.x;
            _direction.z = walkingSpeed * _direction.z;
            _playerAnimator.SetBool("isRunning", false);
            _heavy.Sprint(false);
        }

        if (_jumpInput && _characterController.isGrounded)
        {
            _direction.y = _jumpHeight;
        }
        else
        {
            _direction.y = directionY;
        }

        if (!_characterController.isGrounded)
        {
            _direction.y -= _gravity * Time.deltaTime;
        }

        _direction = transform.TransformDirection(_direction);
        _characterController.Move(_direction * Time.deltaTime);
    }

    private void FireWeapon()
    {
       if (_primaryFireInput)
        {
            _heavy.PrimaryFire();
        }

        if (_secondaryFireInput)
        {
            _heavy.SecondaryFire();
        }
    }
}