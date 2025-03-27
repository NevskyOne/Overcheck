using UnityEngine;
using Zenject;

public class StampButton : MonoBehaviour, IInteractable
{
    [SerializeField] private CheckState _checkState;
    [SerializeField] private Material _activeMaterial;
    [SerializeField] private Material _passiveMaterial;
    [SerializeField] private StampButton _anotherButton;

    private MeshRenderer _meshRenderer;
    public int CursorInd { get; set; } = 1;

    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _meshRenderer.material = _passiveMaterial;
        eventBus.Subscribe((NewDayStartedEvent _) => { Uninteract();});
    }

    public void Interact()
    {
        if (Player.CheckingState == _checkState)
        {
            Uninteract();
            return;
        }
        _anotherButton.Uninteract();
        
        _meshRenderer.material = _activeMaterial;

        Player.CheckingState = _checkState;
    }

    public void Uninteract()
    {
        Player.CheckingState = CheckState.None;
        _meshRenderer.material = _passiveMaterial;
    }

    private void OnDisable()
    {
        _meshRenderer.material = _activeMaterial;
    }
}
