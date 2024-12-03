using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField][Range(0,5f)] private float shakeAmplitude = 0.1f; 
    [SerializeField][Range(0,5f)] private float shakeFrequency = 5f;
    [Header("Settings")]
    [SerializeField][Range(0,1f)] private float _smoothTime;
    [SerializeField][Range(0,1f)] private float _transitionTime;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private Vector2 rotationXLimits;
    [Header("Objects")]
    [SerializeField] private Camera _cam;
    
    private PlayerInput _input => GetComponent<PlayerInput>();
    private PlayerSFX _sfx => GetComponent<PlayerSFX>();
    private Effects _fx => FindFirstObjectByType<Effects>();
    private Vector3 _newPos, _newRot, _camRot;
    private Vector3 _velocity = Vector3.zero;
    private float _speed, _fov = 60, _refTransition, _refZRotate;
    
    private float _mouseSens => SettingsUI.MouseSens;
    
    private void Awake()
    {
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
        _sfx.PlayBreath();
        StartCoroutine(_fx.ChangeChromatic(1));
    }
    
    private void StopSprint(InputAction.CallbackContext _)
    {
        _speed = _maxSpeed;
        _fov = 60;
        _sfx.PlayBreath(false);
        StartCoroutine(_fx.ChangeChromatic(0.1f));
    }
    
    private void Look(InputAction.CallbackContext _)
    {
        var delta = _input.actions["Look"].ReadValue<Vector2>();
        var camAngles = _cam.transform.eulerAngles;
        _newRot = new Vector3(0, transform.eulerAngles.y + delta.x * _mouseSens, 0);
        
        _camRot = new Vector3(Mathf.Clamp(NormalizeAngle(camAngles.x - delta.y * _mouseSens),
            rotationXLimits.x, rotationXLimits.y),0, camAngles.z);
    }

    private void FixedUpdate()
    {
        var delta = _input.actions["Move"].ReadValue<Vector2>();
        var direction = new Vector3(delta.x, 0, delta.y); // Вектор ввода
        direction = Quaternion.Euler(0, transform.eulerAngles.y, 0) * direction; // Учет поворота игрока
        _newPos = transform.position + direction.normalized * (_speed * Time.fixedDeltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, _newPos, ref _velocity, _smoothTime);
        
        _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView, _fov, ref _refTransition, _transitionTime);

        ApplyShake(_velocity.magnitude);
        transform.eulerAngles = _newRot;
        _cam.transform.localEulerAngles = _camRot;
        
    }

    private void ApplyShake(float movementSpeed)
    {
        if (movementSpeed > 0.1f)
        {
            float shakeAmount = Mathf.Sin(Time.time * shakeFrequency * Mathf.PI * 2) * shakeAmplitude;
            _camRot.z = shakeAmount;
            _sfx.PlayFeet();
        }
        else
        {
            _camRot.z = 0f;
            _sfx.PlayFeet(false);
        }

    }
    
    private float NormalizeAngle(float angle)
    {
        angle %= 360;
        return angle > 180 ? angle - 360 : angle;
    }
}
