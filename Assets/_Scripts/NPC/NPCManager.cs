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
    [Header("Randomnes")] 
    public uint CriminalChance;
    [SerializeField] private uint _eventChance;
    [Header("NPC")] 
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
    private List<string> _criminals  = new();
    public List<string> ChangedCriminals { get; private set; } = new();

    private Random _rnd = new Random();
    private NavMeshAgent _currentAgent;
    private int _weekDate => FindFirstObjectByType<TimeLines>().WeekDate;
    private DayNPC _currentDay;
    private bool _isChecked;
    
    public static event Action OnNPCEnd, RandomEvent, EternityCheck;
    

    private void Start()
    {
        var names = RandomParamSt.Names.ToList();
        for (int i = 0; i < _rnd.Next(3, 5); i++)
        {
            name = names[_rnd.Next(names.Count)];
            _criminals.Add(name);
            names.Remove(name);
        }

        ChangedCriminals = _criminals;
        RandomEvents.OnLose += ResetDay;
    }
    
    void Update()
    {
        if (_isChecked && _currentAgent && _currentAgent.velocity.magnitude < 0.1f)
        {
            
            SelectNPC();
            _isChecked = false;
        }
    }

    public void StartDay()
    {
        _criminals = ChangedCriminals;
        _currentDay = _npcList[_weekDate];
        SelectNPC();
        int i = 0;
        foreach (var criminal in _criminals)
        {
            _criminalHolder.GetChild(i).GetComponent<TMP_Text>().text = criminal;
            i++;
        }
    }

    public void ResetDay()
    {
        ChangedCriminals = _criminals;
        _currentDay = _npcList[_weekDate];
    }

    public void SelectNPC(bool eventEnabled = true)
    { 
        if(CurrentNPC) Destroy(CurrentNPC.gameObject);
        
        if(eventEnabled && _rnd.Next(0,101) < _eventChance && (_currentDay.TutorialNPC > 0 || _currentDay.SpecialNPC > 0 || _currentDay.NormalNPC > 0))
           RandomEvent?.Invoke();
        else
        {
            if (_currentDay.TutorialNPC > 0)
            {
                SpawnNPC(_tutorialNPC);
                _currentDay.TutorialNPC -= 1;
            }
            else if (_rnd.Next(2) == 1 && _currentDay.SpecialNPC > 0)
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
    }

    private void SpawnNPC(List<GameObject> npcList)
    {
        _currentAgent = Instantiate(npcList[_rnd.Next(0, npcList.Count)], _startPos.position, Quaternion.identity).GetComponent<NavMeshAgent>();
        _currentAgent.SetDestination(_tablePos.position);
        CurrentNPC = _currentAgent.GetComponent<NPC>();
    }

    public async void GoBack()
    {
        if(CurrentNPC.NPCTimeLine == TimeLine.Eternity)
            EternityCheck?.Invoke();
        _currentAgent.SetDestination(_startPos.position);
        await Task.Delay(_rnd.Next(2000, 7000));
        _isChecked = true;
    }
    
    public async void GoTowards()
    {
        if(CurrentNPC.NPCTimeLine == TimeLine.Eternity)
            EternityCheck?.Invoke();
        _currentAgent.SetDestination(_endPos.position);
        await Task.Delay(_rnd.Next(2000, 7000));
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
