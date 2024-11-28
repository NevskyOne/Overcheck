using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class NPCManager : MonoBehaviour
{
    [Header("NPC")] 
    public uint CriminalChance;
    [SerializeField] private List<DayNPC> _npcList = new(7);
    [SerializeField] private List<GameObject> _normalNPC;
    [SerializeField] private List<GameObject> _specialNPC;
    [SerializeField] private List<GameObject> _tutorialNPC;
    
    [Header("DocsSpawn")] 
    [SerializeField] private Transform _pos1;
    [SerializeField] private Transform _pos2;
    [SerializeField] private Transform _pos3;

    [Header("NPCPoints")] 
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _tablePos;
    [SerializeField] private Transform _endPos;

    [Header("UI")] 
    [SerializeField] private Transform _criminalHolder;

    public NPC CurrentNPC {get; private set; }
    public List<string> Criminals { get; } = new();
    

    private Random _rnd = new Random();
    private NavMeshAgent _currentAgent;
    private int _weekDate => FindFirstObjectByType<TimeLines>().WeekDate;
    private DayNPC _currentDay;
    private bool _isChecked;
    
    public event Action OnNPCEnd;
    

    private void Start()
    {
        var names = RandomParamSt.Names.ToList();
        for (int i = 0; i < _rnd.Next(3, 5); i++)
        {
            name = names[_rnd.Next(names.Count)];
            Criminals.Add(name);
            names.Remove(name);
        }
    }
    
    void Update()
    {
        if (_isChecked && _currentAgent && _currentAgent.isStopped)
        {
            SelectNPC();
            _isChecked = false;
        }
    }

    public void StartDay()
    {
        _currentDay = _npcList[_weekDate];
        SelectNPC();
        int i = 0;
        foreach (var criminal in Criminals)
        {
            _criminalHolder.GetChild(i).GetComponent<TMP_Text>().text = criminal;
            i++;
        }
    }
    
    private async void SelectNPC()
    { 
        if(CurrentNPC) Destroy(CurrentNPC.gameObject);
        await Task.Delay(_rnd.Next(2000, 7000));
        if (_currentDay.TutorialNPC > 0)
        {
            SpawnNPC(_tutorialNPC);
            _currentDay.TutorialNPC -= 1;
        }
        else if (_rnd.Next(0, 2) == 1 && _currentDay.SpecialNPC > 0)
        {
            SpawnNPC(_specialNPC);
            _currentDay.SpecialNPC -= 1;
        }
        else if (_currentDay.NormalNPC > 0)
        {
            SpawnNPC(_normalNPC);
            _currentDay.NormalNPC -= 1;
        }
        else
            OnNPCEnd?.Invoke();
    }

    private void SpawnNPC(List<GameObject> npcList)
    {
        _currentAgent = Instantiate(npcList[_rnd.Next(0, npcList.Count)], _startPos.position, Quaternion.identity).GetComponent<NavMeshAgent>();
        _currentAgent.SetDestination(_tablePos.position);
        CurrentNPC = _currentAgent.GetComponent<NPC>();
    }

    public void GoBack()
    {
        _currentAgent.SetDestination(_startPos.position);
        _isChecked = true;
    }
    
    public void GoTowards()
    {
        _currentAgent.SetDestination(_endPos.position);
        _isChecked = true;
    }
    
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

[Serializable]
public struct DayNPC
{
    public uint NormalNPC;
    public uint SpecialNPC;
    public uint TutorialNPC;
}
