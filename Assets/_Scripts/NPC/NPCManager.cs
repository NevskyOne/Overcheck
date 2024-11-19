using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class NPCManager : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private List<GameObject> _normalNPC;
    [SerializeField] private List<GameObject> _specialNPC;
    [SerializeField] private List<GameObject> _educateNPC;

    [Header("DocsSpawn")] 
    [SerializeField] private Transform _pos1;
    [SerializeField] private Transform _pos2;
    [SerializeField] private Transform _pos3;

    [Header("NPCPoints")] 
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _tablePos;
    [SerializeField] private Transform _endPos;

    private Random _rnd = new Random();
    private NavMeshAgent _currentNPC;
    
    private void Start()
    {
        _currentNPC = Instantiate(_normalNPC[_rnd.Next(0, _normalNPC.Count)], _startPos.position, Quaternion.identity).GetComponent<NavMeshAgent>();
        _currentNPC.SetDestination(_tablePos.position);
    }

    public void GoBack()
    {
        _currentNPC.SetDestination(_startPos.position);
    }
    
    public void GoTowards()
    {
        _currentNPC.SetDestination(_endPos.position);
    }

    public void NPCGiveDocs() => _currentNPC.GetComponent<NPC>().GiveDocs();
    public void NPCCheck(Goal playerGoal, bool playerOrigin) => _currentNPC.GetComponent<NPC>()
        .CheckCoast(playerGoal,playerOrigin);
    
    public Document GiveDoc(GameObject doc, uint pos)
    {
        return pos switch
        {
            1 => Instantiate(doc, _pos1.position, Quaternion.identity).GetComponent<Document>(),
            2 => Instantiate(doc, _pos2.position, Quaternion.identity).GetComponent<Document>(),
            3 => Instantiate(doc, _pos3.position, Quaternion.identity).GetComponent<Document>(),
            _ => null
        };
    }
}
