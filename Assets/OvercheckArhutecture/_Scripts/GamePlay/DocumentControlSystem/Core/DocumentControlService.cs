using System.Collections;
using UnityEngine;
using Zenject;

public class DocumentControlService : MonoBehaviour
{
    [SerializeField] private Transform _spawnpoint;
    [SerializeField] private Transform _documentcheckpoint;
    [SerializeField] private Transform _exitpoint;
    [SerializeField] private Transform _continuepoint;
    
    private EventBus _eventBus;
    private NPCBase _currentNPC;
    private DayData _currentDay;
    private bool _isGameStarted;
    private bool _isInControl;
    private int _currentNpcIndex;

    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _eventBus = eventBus;

        _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        _eventBus.Subscribe<GamePausedEvent>(OnGamePaused);
        _eventBus.Subscribe<NewDayStartedEvent>(OnNewDayStarted);
        _eventBus.Subscribe<NPCSpawnedEvent>(OnNPCSpawned);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartControl();
            Approve();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            StartControl();
            Decline();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            StartProcess();
            StartControl();
        }
    }

    private void StartProcess()
    {
        var data = _currentDay.NPCs[_currentNpcIndex];
        _eventBus.Invoke(new SpawnNPCRequestEvent(new NPCSpawnData(data, _spawnpoint.position)));
    }
    
    private void OnNPCSpawned(NPCSpawnedEvent e)
    {
        if (e.SpawnData.SpawnPosition != _spawnpoint.position)
            return;
        
        _currentNPC = e.NPC;
        _currentNPC.GoToPoint(_documentcheckpoint.position);
    }

    public void StartControl()
    {
        _isInControl = true;
    }
    
    public void Approve()
    {
        if (!_isInControl || !_isGameStarted) return;
        
        _isInControl = false;
        _eventBus.Invoke(new NPCControledEvent(_currentNPC.NPCData, true));
        StartCoroutine(ProcessRoutine(_continuepoint.position));
    }

    public void Decline()
    {
        if (!_isInControl || !_isGameStarted) return;
        
        _isInControl = false;
        _eventBus.Invoke(new NPCControledEvent(_currentNPC.NPCData, false));
        StartCoroutine(ProcessRoutine(_exitpoint.position));
    }

    private IEnumerator ProcessRoutine(Vector3 point)
    {
        _currentNPC.GoToPoint(point);
        while (!_currentNPC.IsInPoint(point))
        {
            yield return null;
        }
        
        Destroy(_currentNPC.gameObject);
        if (_currentNpcIndex < _currentDay.NPCs.Count)
        {
            _currentNpcIndex++;
            StartProcess();
        }
        else
            _eventBus.Invoke(new AllDayNPCsEndedEvent());
    }
    
    private void OnNewDayStarted(NewDayStartedEvent e)
    {
        _currentDay = e.DayData;
        _currentNpcIndex = 0;
    }
    
    private void OnGameStarted(GameStartedEvent e)
    {
        _isGameStarted = true;
    }

    private void OnGamePaused(GamePausedEvent e)
    {
        _isGameStarted = false;
    }
}