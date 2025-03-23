using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(NavMeshAgent), typeof(NPCAnim))]
public abstract class NPCBase : MonoBehaviour, IInteractable
{
    private NavMeshAgent _agent;
    private NPCAnim _animator;
    
    private NPCData _npcData;
    private DialogConfig _dialog;
    private DialogSystem _dialogSystem;
    
    public CheckState State { get; set; } = CheckState.None;
    public int CursorInd { get; set; } = 2;
    
    public NPCData NPCData => _npcData;

    [Inject]
    public void Initialize(DialogSystem dialogSystem)
    {
        _dialogSystem = dialogSystem;

        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<NPCAnim>();
    }
    
    public void Setup(NPCData npcData, NPCService npcService)
    {
        
        _npcData = npcData;
        var appearence = _npcData.NpcAppearanceData;
        var appearStruct = npcService.AppearStruct;
        
        var meshRenderer = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();
        meshRenderer.sharedMesh = appearStruct.Models[appearence.Model];
        meshRenderer.SetMaterials(new()
        {
            appearStruct.Materials[appearence.ModelMaterial],
            appearStruct.Materials[appearence.ModelMaterial],
            appearStruct.AccessMaterials[appearence.AccessMaterial]
        });
        foreach (var i in npcData.NpcAppearanceData.Accessories)
            meshRenderer.SetBlendShapeWeight(i, 100);

        SetDialog(_npcData.Config);
    }

    public void SetDialog(DialogConfig config) => _dialog = config;

    public void Interact()
    {
        switch (State)
        {
            case CheckState.None:
                _dialogSystem.FragmentsStack = new List<DialogFragment>(_dialog.Fragments);
                _dialogSystem.SetNPCAnim(_animator);
                _dialogSystem.PlayNext();
                break;
            case CheckState.Correct:
                _dialogSystem.FragmentsStack = new List<DialogFragment>{_dialog.GoFragment};
                _dialogSystem.SetNPCAnim(_animator);
                _dialogSystem.PlayNext();
                DialogSystem.GoAfter = CheckState.Correct;
                break;
            case CheckState.Wrong:
                _dialogSystem.FragmentsStack = new List<DialogFragment>{_dialog.GoFragment};
                _dialogSystem.SetNPCAnim(_animator);
                _dialogSystem.PlayNext();
                DialogSystem.GoAfter = CheckState.Wrong;
                break;
        }
    }
    
    public void Uninteract()
    {
        _dialogSystem.EndChat();
    }

    public void GoToPoint(Vector3 point)
    {
        GetComponent<NavMeshAgent>().SetDestination(point);
    }

    public bool IsInPoint(Vector3 point)
    {
        var agentPos = new Vector3(transform.position.x, 0, transform.position.z);
        var finalPoint = new Vector3(point.x, 0, point.z);
        
        return Vector3.Distance(agentPos, finalPoint) <= _agent.stoppingDistance;
    }
}