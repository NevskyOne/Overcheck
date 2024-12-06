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
    public uint EventChance = 30;
    [Header("NPC")] 
    public List<DayNPC> NpcList = new(7);
    [SerializeField] private List<GameObject> _normalNPC;
    [SerializeField] private List<GameObject> _eternityNPC;
    [SerializeField] private List<GameObject> _agentsNPC;
    [SerializeField] private List<GameObject> _robotsNPC;
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

    public static NPC CurrentNPC {get; private set; }
    private List<string> _criminals  = new();
    public List<string> ChangedCriminals { get; private set; } = new();

    private Random _rnd = new Random();
    
    private NavMeshAgent _currentAgent;
    private NPCAnim _npcAnim;
    
    private int _weekDate => FindFirstObjectByType<TimeLines>().WeekDate;
    private DayNPC _currentDay;
    private bool _isChecked, _toTable;
    
    public static event Action OnNPCEnd, RandomEvent, EternityCheck, NPCAtTable, OnNPCCheck;
    

    private void Start()
    {
        var names = RandomParamSt.Names.ToList();
        for (int i = 0; i < _rnd.Next(3, 5); i++)
        {
            var bearName = names[_rnd.Next(names.Count)];
            _criminals.Add(bearName);
            names.Remove(bearName);
        }

        ChangedCriminals = _criminals;
        RandomEvents.OnLose += ResetDay;
    }
    
    void Update()
    {
        if (!_currentAgent || !(_currentAgent.velocity.magnitude < 0.1f)) return;
        if (_isChecked)
        {
            SelectNPC();
            _isChecked = false;
        }

        else if (_toTable)
        {
            NPCAtTable?.Invoke();
            _npcAnim.TurnRight();
            _toTable = false;
        }
    }

    public void StartDay()
    {
        _criminals = ChangedCriminals;
        _currentDay = NpcList[_weekDate];
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
        _currentDay = NpcList[_weekDate];
    }

    public void SetNPCTalking(bool talking = true)
    {
        if(_npcAnim)
            _npcAnim.IsTalking = talking;
    }

    public void SelectNPC(bool eventEnabled = true)
    { 
        if(CurrentNPC) Destroy(CurrentNPC.gameObject);
        var randomSpecial = _rnd.Next(3);
        if(eventEnabled && _rnd.Next(0,101) < ((SettingsUI.RobotsCount ^ 2) * 2 + EventChance) &&
           (_currentDay.TutorialNPC > 0 || _currentDay.EternityNPC > 0 ||
            _currentDay.AgentNPC > 0 || _currentDay.RobotsNPC > 0 || _currentDay.NormalNPC > 0))
            RandomEvent?.Invoke();
        else
        {
            if (_currentDay.TutorialNPC > 0)
            {
                SpawnNPC(_tutorialNPC);
                _currentDay.TutorialNPC -= 1;
            }
            else if (randomSpecial == 0 && _currentDay.EternityNPC > 0)
            {
                SpawnNPC(_eternityNPC);
                _currentDay.EternityNPC -= 1;
            }
            else if (randomSpecial == 1 && _currentDay.AgentNPC > 0)
            {
                SpawnNPC(_agentsNPC);
                _currentDay.AgentNPC -= 1;
            }
            else if (randomSpecial == 2 && _currentDay.RobotsNPC > 0)
            {
                SpawnNPC(_robotsNPC);
                _currentDay.RobotsNPC -= 1;
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

    private async void SpawnNPC(List<GameObject> npcList)
    {
        _currentAgent = Instantiate(npcList[_rnd.Next(0, npcList.Count)], _startPos.position, Quaternion.identity).GetComponent<NavMeshAgent>();
        _currentAgent.SetDestination(_tablePos.position);
        CurrentNPC = _currentAgent.GetComponent<NPC>();
        _npcAnim = CurrentNPC.GetComponent<NPCAnim>();
        await Task.Delay(1000);
        _toTable = true;
    }

    public async void GoBack()
    {
        _npcAnim.TurnRight();
        
        OnNPCCheck?.Invoke();
        if(CurrentNPC.NPCTimeLine == TimeLine.Eternity)
            EternityCheck?.Invoke();
        
        _currentAgent.SetDestination(_startPos.position);
        await Task.Delay(10000);
        _isChecked = true;
    }
    
    public async void GoTowards()
    {
        _npcAnim.TurnLeft();
        
        OnNPCCheck?.Invoke();
        if(CurrentNPC.NPCTimeLine == TimeLine.Eternity)
            EternityCheck?.Invoke();
        
        _currentAgent.SetDestination(_endPos.position);
        await Task.Delay(10000);
        _isChecked = true;
    }
    
    public Document GiveDoc(GameObject doc, uint pos)
    {
        return pos switch
        {
            1 => Instantiate(doc, _pos1.position, Quaternion.Euler(0,90,0))
                .GetComponent<Document>(),
            2 => Instantiate(doc, _pos2.position, Quaternion.Euler(0,90,0)).GetComponent<Document>(),
            3 => Instantiate(doc, _pos3.position,Quaternion.Euler(0,90,0)).GetComponent<Document>(),
            _ => null
        };
    }
}

[Serializable]
public struct DayNPC
{
    public uint NormalNPC;
    public uint EternityNPC;
    public uint AgentNPC;
    public uint RobotsNPC;
    public uint TutorialNPC;
}
