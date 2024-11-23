using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField][Range(0,1f)] private float _positioningTime;
    [SerializeField][Range(0,1f)] private float _rotationTime;
    
    public Vector3 TargetPos { get; set; }
    
    public Vector3 TargetRot { get; set; }

    private Vector3 
        _defaultPos, _defaultRot,
        _refPos = Vector3.zero, _refRot = Vector3.zero;

    private void Start()
    {
        _defaultPos = transform.localPosition;
        _defaultRot = transform.localEulerAngles;
        
        TargetPos = _defaultPos;
        TargetRot = _defaultRot;
    }

    private void FixedUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition,
            TargetPos, ref _refPos, _positioningTime);
        
        transform.localEulerAngles = Vector3.SmoothDamp( transform.localEulerAngles,
            TargetRot, ref _refRot, _rotationTime);

    }

    public void Reset()
    {
        TargetPos = _defaultPos;
        TargetRot = _defaultRot;
    }
    
}
