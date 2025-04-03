using System;
using UnityEngine;

public class PlayerMovement
{
    private readonly MovementStruct _struct;
    
    private readonly Camera _cam;
    private PlayerSFX _sfx => Player.Sfx;
    private float _mouseSens => SettingsUI.MouseSens;
    private readonly VisualEffects _visualFX;
    private readonly Transform _playerTransform;
    
    private Vector3 _newPos, _newRot;
    private Vector3 _velocity = Vector3.zero;
    private float _speed, _fov = 60, _refTransition, _refZRotate;
    private bool enabled = true;
    private EventBus _eventBus;

    public Vector3 CamRot;
    private MainUI _mainUI;
    private bool _fRun, _isSprinting, _sprintActive;
    
    public static event Action OnRun, OnRunEnd;


    public PlayerMovement(MovementStruct movementStruct, VisualEffects effects, Camera cam, Transform transform, MainUI mainUI, EventBus eventBus)
    {
        _struct = movementStruct;
        _visualFX = effects;
        _cam = cam;
        _playerTransform = transform;

        _mainUI = mainUI;

        _eventBus = eventBus;
        
        _speed = _struct.MaxSpeed;
    }

    
    public void Enable()
    {
        _visualFX.ChangeChromatic(0.05f);
        enabled = true;
    }

    public void Disable()
    {
        _visualFX.ChangeChromatic(0.05f);
        _sfx.PlayBreath(false);
        _sfx.PlayFeet(false);
        enabled = false;
    }
    
    public void StartSprint()
    {
        _speed = _struct.MaxSpeed * 1.5f;
        _fov = 75;
        _sfx.PlayBreath();
        OnRun?.Invoke();
        _visualFX.ChangeChromatic(0.5f);
    }
    
    public void StopSprint()
    {
        _speed = _struct.MaxSpeed;
        _fov = 60;
        _sfx.PlayBreath(false);
        OnRunEnd?.Invoke();
        _visualFX.ChangeChromatic(0.05f);
    }
    
    public void Look(Vector2 delta)
    {
        var camAngles = _cam.transform.eulerAngles;
        _newRot = new Vector3(0, _playerTransform.eulerAngles.y + delta.x * _mouseSens, 0);
        
        CamRot = new Vector3(Mathf.Clamp(NormalizeAngle(camAngles.x - delta.y * _mouseSens),
            _struct.RotationLimits.x, _struct.RotationLimits.y),0, 0);
    }
    
    public void LocalUpdate(Vector2 delta)
    {
        if(!enabled) return;

        if (delta != Vector2.zero)
        {
            if (!_fRun)
            {
                _struct.Anim.SetFloat("Forward", 1);
                _fRun = true;
            }

            if (_isSprinting && !_sprintActive)
            {
                _fov = 75;
                _sfx.PlayBreath();
                _visualFX.ChangeChromatic(0.5f);
                _sprintActive = true;
                _mainUI.DrainRun();
            }
            else if(!_isSprinting)
            {
                _fov = 60;
                _sfx.PlayBreath(false);
                _visualFX.ChangeChromatic(0.05f);
                _sprintActive = false;
                _mainUI.FillRun();
            }
        }
        else
        {
            _struct.Anim.SetFloat("Forward", 0);
            _fRun = false;
        }
        
        var direction = new Vector3(delta.x, 0, delta.y); // Вектор ввода
        direction = Quaternion.Euler(0, _playerTransform.eulerAngles.y, 0) * direction; // Учет поворота игрока
        _newPos = _playerTransform.position + direction.normalized * (_speed * Time.fixedDeltaTime);
        _playerTransform.position = Vector3.SmoothDamp(_playerTransform.position, _newPos, ref _velocity, _struct.SmoothTime);
        
        _cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView, _fov, ref _refTransition, _struct.TransitionTime);

        ApplyShake(_velocity.magnitude);
        _playerTransform.eulerAngles = _newRot;
        _cam.transform.localEulerAngles = CamRot;
    }

    private void ApplyShake(float movementSpeed)
    {
        if (movementSpeed > 0.1f)
        {
            float shakeAmount = _cam.transform.localPosition.y
                                + Mathf.Cos(Time.fixedTime * _struct.ShakeFrequency * Mathf.PI * 2)
                                * _struct.ShakeAmplitude;
            _cam.transform.localPosition = new Vector3(0,shakeAmount,0);
            _sfx.PlayFeet();
        }
        else
        {
            _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition,new Vector3(0,0.7f,0), Time.fixedDeltaTime);
            _sfx.PlayFeet(false);
        }

    }
    
    private float NormalizeAngle(float angle)
    {
        angle %= 360;
        return angle > 180 ? angle - 360 : angle;
    }
}
