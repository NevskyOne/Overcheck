
using UnityEngine;

public class CamMove : MonoBehaviour
{
    [SerializeField] private Transform _camPos;
    [SerializeField] private Vector3 _camRot;
    
    private CameraManager _cameraMng;
    private void Start() => _cameraMng = FindFirstObjectByType<CameraManager>();
    
    public void Move(Vector3 playerRot)
    {
        _cameraMng.MoveToTarget(_camPos.position,_camRot - playerRot);
    }
}
