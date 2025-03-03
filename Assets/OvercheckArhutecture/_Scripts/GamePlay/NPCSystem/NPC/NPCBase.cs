using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class NPCBase : MonoBehaviour
{
    private NavMeshAgent _agent;
    private NPCData _npcData;

    public NPCData NPCData => _npcData;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    
    public void Setup(NPCData npcData)
    {
        _npcData = npcData;
        // здесь реализация сборки нпс. Т.е. его внешности и т.д.
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