using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StartButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Material _activeMaterial;
    [SerializeField] private Material _passiveMaterial;
    
    private bool enable;
    private DocumentControlService _docsService;

    private MeshRenderer _meshRenderer => GetComponent<MeshRenderer>();
    public int CursorInd { get; set; } = 5;
    
    public bool Enabled
    {
        get
        {
            return enable;
        }
        set
        {
            _meshRenderer.material = value? _passiveMaterial : _activeMaterial;
            enable = value;
        }
    }

    [Inject]
    private void Initialize(DocumentControlService service, EventBus eventBus)
    {
        _meshRenderer.material = _activeMaterial;
        _docsService = service;
        eventBus.Subscribe((NewDayStartedEvent _) => { Uninteract();});
    }

    public void Interact()
    {
        Enabled = true;
        _docsService.StartProcess();
        _docsService.StartControl();
    }
    public void Uninteract(){Enabled = false;}

    private void OnDisable()
    {
        Uninteract();
    }
}
