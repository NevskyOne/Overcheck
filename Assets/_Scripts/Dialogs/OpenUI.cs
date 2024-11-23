
using UnityEngine;

public class OpenUI : MonoBehaviour
{
    private PlayerInteractions _playerInteract;
    private void Start() => _playerInteract = FindFirstObjectByType<PlayerInteractions>();

    private void OnEnable() => _playerInteract.OnUIClick += Open;
    private void OnDisable() => _playerInteract.OnUIClick -= Open;
    
    private void Open(){}
}
