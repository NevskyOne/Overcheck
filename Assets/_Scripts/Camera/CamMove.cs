
using System;
using UnityEngine;
using Zenject;

public class CamMove : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _camPos;
    
    private CameraManager _cameraMng;
    protected Player _player;
    protected EventBus _eventBus;
    protected MainUI _mainUI;

    public Vector3 HitPos;
    public event Action OnInteract; 
    public int CursorInd { get; set; } = 1;
    
    [Inject]
    private void Initialize(CameraManager cameraManager, Player player, EventBus bus, MainUI mainUI)
    {
        _cameraMng = cameraManager;
        _player = player;
        _eventBus = bus;
        _mainUI = mainUI;
    }

    public virtual void Interact()
    {
        ChangePlayerState();
        if (HitPos == Vector3.zero)
            HitPos = _camPos.position;
        _cameraMng.MoveToTarget(_camPos.position,_camPos.eulerAngles - _player.transform.eulerAngles, HitPos);
        Player.Interactions.StopFocus();
        OnInteract?.Invoke();
        HitPos = Vector3.zero;
    }

    protected virtual void ChangePlayerState() => Player.State = PlayerState.UI;
    
    public virtual void Uninteract()
    {
        _cameraMng.ResetCamera();
        _mainUI.CloseMenus();
    }
}
