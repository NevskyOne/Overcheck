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
    private DocumentControlService _docControl;
    
    public CheckState State { get; set; } = CheckState.None;
    
    public NPCData NPCData => _npcData;

    [Inject]
    public void Initialize(DialogSystem dialogSystem, DocumentControlService docControl)
    {
        _dialogSystem = dialogSystem;
        _docControl = docControl;
    }
    
    public void Setup(NPCData npcData, NPCService npcService)
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<NPCAnim>();
        
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

        _dialog = _npcData.Config;
    }

    public void Interact()
    {
        switch (State)
        {
            case CheckState.None:
                _dialogSystem.FragmentsStack = _dialog.Fragments;
                _dialogSystem.SetNPCAnim(_animator);
                _dialogSystem.PlayNext();
                break;
            case CheckState.Correct:
                _docControl.Accept();
                break;
            case CheckState.Wrong:
                _docControl.Reject();
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