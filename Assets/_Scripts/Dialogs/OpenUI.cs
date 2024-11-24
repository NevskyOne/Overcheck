
using UnityEngine;

public class OpenUI : MonoBehaviour
{
    [SerializeField] private Transform _camPos;
    [SerializeField] private Vector3 _camRot;
    [SerializeField] private GameObject _UI;
    
    private CameraManager _cameraMng;
    private void Start() => _cameraMng = FindFirstObjectByType<CameraManager>();
    
    public void Open(Vector3 playerRot)
    {
        _cameraMng.MoveToTarget(_camPos.position,_camRot - playerRot);
        _UI.SetActive(true);
    }
}
