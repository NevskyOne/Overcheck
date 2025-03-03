
using System;
using UnityEngine;
using Zenject;

public class CamMove : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _camPos;
    
    private CameraManager _cameraMng;
    protected Player _player;
    public event Action OnInteract; 
    
    [Inject]
    private void Initialize(CameraManager cameraManager, Player player)
    {
        _cameraMng = cameraManager;
        _player = player;
    }

    public virtual void Interact()
    {
        _cameraMng.MoveToTarget(_camPos.position,_camPos.eulerAngles - _player.transform.eulerAngles);
        ChangePlayerState();
        OnInteract?.Invoke();
    }

    protected virtual void ChangePlayerState() => Player.State = PlayerState.UI;
    
    public void Uninteract()
    {
        _cameraMng.ResetCamera();
    }
}
