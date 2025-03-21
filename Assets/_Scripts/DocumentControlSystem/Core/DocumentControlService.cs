using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DocumentControlService : MonoBehaviour
{
    [Header("Documents")]
    [SerializeField] private Document[] _documents;
    [SerializeField] private Transform _docSpawnpoint;
    [Header("NPC")]
    [SerializeField] private Transform _spawnpoint;
    [SerializeField] private Transform _documentcheckpoint;
    [SerializeField] private Transform _exitpoint;
    [SerializeField] private Transform _continuepoint;
    
    private EventBus _eventBus;
    private RandomEvents _randomEvents;
    private NPCBase _currentNPC;
    private DayData _currentDay;
    private bool _isGameStarted;
    private bool _isInControl;
    private int _currentNpcIndex;
    private Document PMS, IIC, PP;
    
    [Inject] private DiContainer _container;
    
    public NPCBase CurrentNPC => _currentNPC;

    [Inject]
    private void Initialize(EventBus eventBus, RandomEvents randomEvents)
    {
        _randomEvents = randomEvents;
        _eventBus = eventBus;

        _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        _eventBus.Subscribe<GamePausedEvent>(OnGamePaused);
        _eventBus.Subscribe<NewDayStartedEvent>(OnNewDayStarted);
        _eventBus.Subscribe<NPCSpawnedEvent>(OnNPCSpawned);
    }
    

    public void StartProcess()
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
    
    public void Accept()
    {
        if (!_isInControl || !_isGameStarted) return;
        DestroyDocs();

        _eventBus.Invoke(new NPCControledEvent(_currentNPC.NPCData, true));
        StartCoroutine(ProcessRoutine(_continuepoint.position));
    }

    public void Reject()
    {
        if (!_isInControl || !_isGameStarted) return;
        DestroyDocs();

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
        if(_randomEvents.ChooseRandomEvent()) yield break;
        
        if (_currentNpcIndex < _currentDay.NPCs.Count - 1)
        {
            _currentNpcIndex++;
            StartProcess();
        }
        else
            _eventBus.Invoke(new AllDayNPCsEndedEvent());
    }
    
    public async void GiveDocs()
    {
        var docsData = _currentNPC.NPCData.DocsData;
        if (DaysService.CurrentDay > 3)
        {
            PP = _container.InstantiatePrefab(_documents[docsData.Docs[2].Document],
                _docSpawnpoint.position, Quaternion.Euler(0,180,0), null).GetComponent<Document>();
            PP.Setup(docsData.Docs[2]);
            await Task.Delay(200);
        }
        if (DaysService.CurrentDay > 1)
        {
            IIC = _container.InstantiatePrefab(_documents[docsData.Docs[1].Document],
                _docSpawnpoint.position, Quaternion.Euler(0,180,0), null).GetComponent<Document>();
            IIC.Setup(docsData.Docs[1]);
            await Task.Delay(200);
        }
        PMS = _container.InstantiatePrefab(_documents[docsData.Docs[0].Document],
            _docSpawnpoint.position, Quaternion.Euler(0,180,0), null).GetComponent<Document>();
        PMS.Setup(docsData.Docs[0]);
    }

    private void DestroyDocs()
    {
        Destroy(PMS?.gameObject);
        Destroy(IIC?.gameObject);
        Destroy(PP?.gameObject);
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