using System;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private float _mouseSens => SettingsUI.MouseSens;
    
    [SerializeField][Range(0,1f)] private float _smoothTime;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private Camera _cam;
    [SerializeField] private Vector2 rotationXLimits;
    [SerializeField][Range(0,1f)] private float _transitionTime;
    
    private PlayerInput _input;
    private Vector3 _newPos, _newRot, _camRot;
    private Vector3 _velocity = Vector3.zero;
    private float _speed, _fov = 60, _refTransition;
    
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
        _speed = _maxSpeed;
    }

    private void OnEnable()
    {
        _input.actions["Look"].performed += Look;
        _input.actions["Sprint"].started += StartSprint;
        _input.actions["Sprint"].canceled += StopSprint;
    }

    private void OnDisable()
    {
        _input.actions["Look"].performed -= Look;
        _input.actions["Sprint"].started -= StartSprint;
        _input.actions["Sprint"].canceled -= StopSprint;
    }
    
    private void StartSprint(InputAction.CallbackContext _)
    {
        _speed = _maxSpeed * 1.5f;
        _fov = 75;
    }
    
    private void StopSprint(InputAction.CallbackContext _)
    {
        _speed = _maxSpeed;
        _fov = 60;
    }
    
    private void Look(InputAction.CallbackContext _)
    {
        var delta = _input.actions["Look"].ReadValue<Vector2>();
        var camAngles = _cam.transform.eulerAngles;
        _newRot = new Vector3(0, transform.eulerAngles.y + delta.x * _mouseSens, 0);
        
        _camRot = new Vector3(Mathf.Clamp(NormalizeAngle(camAngles.x - delta.y * _mouseSens),
            rotationXLimits.x, rotationXLimits.y),0, 0);
    }

    private void FixedUpdate()
    {
        var delta = _input.actions["Move"].ReadValue<Vector2>();
        var direction = new Vector3(delta.x, 0, delta.y); // Вектор ввода
        direction = Quaternion.Euler(0, transform.eulerAngles.y, 0) * direction; // Учет поворота игрока
        _newPos = transform.position + direction.normalized * (_speed * Time.fixedDeltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, _newPos, ref _velocity, _smoothTime);
        
        _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView, _fov, ref _refTransition, _transitionTime);

        transform.eulerAngles = _newRot;
        _cam.transform.localEulerAngles = _camRot;
    }
    
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        return angle > 180 ? angle - 360 : angle;
    }
}
