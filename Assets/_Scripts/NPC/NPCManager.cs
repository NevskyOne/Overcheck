using System.Collections.Generic;
using System.Threading.Tasks;
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
    private NPC _currentNPC;
    private NavMeshAgent _currentAgent;

    private void Start()
    {
        SpawnNPC();
    }

    private void SpawnNPC()
    {
        _currentAgent = Instantiate(_normalNPC[_rnd.Next(0, _normalNPC.Count)], _startPos.position, Quaternion.identity).GetComponent<NavMeshAgent>();
        _currentAgent.SetDestination(_tablePos.position);
        _currentNPC = _currentAgent.GetComponent<NPC>();
    }

    public async void GoBack()
    {
        _currentAgent.SetDestination(_startPos.position);
        await Task.Delay(3000);
        Destroy(_currentNPC.gameObject);
        await Task.Delay(1000);
        SpawnNPC();
    }
    
    public async void GoTowards()
    {
        _currentAgent.SetDestination(_endPos.position);
        await Task.Delay(3000);
        Destroy(_currentNPC.gameObject);
        await Task.Delay(1000);
        SpawnNPC();
    }

    public void NPCSetGoal(int value) => _currentNPC.CurrentGoal = (Goal)value;
    public void NPCGiveDocs() => _currentNPC.GiveDocs();
    public void NPCCollectDoc() => _currentNPC.CollectDoc();
    public void NPCCheck(bool playerOrigin) => _currentNPC.CheckCoast(playerOrigin);
    
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
