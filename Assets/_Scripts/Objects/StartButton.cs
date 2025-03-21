using System;
using UnityEngine;
using Zenject;

public class StartButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Material _material;
    private bool enable;
    private DocumentControlService _docsService;

    public int CursorInd { get; set; } = 5;
    
    public bool Enabled
    {
        get
        {
            return enable;
        }
        set
        {
            _material.color = value ? Color.green : Color.grey;
            enable = value;
        }
    }

    [Inject]
    private void Initialize(DocumentControlService service)
    {
        _material.color = Color.grey;
        _docsService = service;
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
