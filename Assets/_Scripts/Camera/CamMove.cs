
using UnityEngine;

public class CamMove : MonoBehaviour
{
    [SerializeField] private Transform _camPos;
    
    private CameraManager _cameraMng;
    private void Start() => _cameraMng = FindFirstObjectByType<CameraManager>();
    
    public void Move(Vector3 playerRot)
    {
        _cameraMng.MoveToTarget(_camPos.position,_camPos.eulerAngles - playerRot );
    }
}
