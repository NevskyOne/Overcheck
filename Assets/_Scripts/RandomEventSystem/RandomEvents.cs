using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class RandomEvents : MonoBehaviour
{
    [Header("Events")]
    [Range(0,100)][SerializeField] private int _eventChance;
    [Range(0,600)][SerializeField] private int _eventTime;
    [SerializeField] private List<UnityEvent> _events = new();
    [Header("Robot")]
    [Range(0,100)][SerializeField] private int _robotChance;
    [SerializeField] private GameObject _robot;

    [Header("Additional")] 
    [SerializeField] private Rigidbody _camRb;
    [SerializeField] private Transform _bedPos;
    [SerializeField] private ScreenFade _blackScreen;
    [SerializeField] private StartButton _startButton;
    
    private UnityEvent _currentEvent;
        
    private MainUI _mainUI;
    private CameraManager _cameraManager;
    private Transform _playerTF;
    private QuizControl _quizControl;
    private DaysService _daysService;
    private DocumentControlService _docControl;
    private Coroutine _loseCoroutine;

    [Inject]
    private void Initialize(MainUI mainUI, CameraManager camMan, Player player,
        QuizControl quizControl, EventBus eventBus, DaysService daysService, DocumentControlService docControl)
    {
        _mainUI = mainUI;
        _playerTF = player.transform;
        _cameraManager = camMan;
        _quizControl = quizControl;
        _daysService = daysService;
        _docControl = docControl;
        
        eventBus.Subscribe<WinEvent>(_ => WinEvent());
        eventBus.Subscribe<LoseEvent>(_ => LoseEvent());
    }
    
    public bool ChooseRandomEvent()
    {
        print("to be chosen");
        if (Random.Range(0, 100) > _eventChance) return false;
        _currentEvent = _events[Random.Range(0, _events.Count)];
        _currentEvent.Invoke();
        print("chosen");
        if(Random.Range(0,100) < _robotChance)
            _robot.SetActive(false);
        _loseCoroutine = StartCoroutine(AutoLose());
        return true;
    }

    private IEnumerator AutoLose()
    {
        int currentTime = 0;
        _mainUI.DrainEvent(_eventTime);
        while (currentTime < _eventTime)
        {
            currentTime++;
            yield return new WaitForSeconds(1);
        }
        
        LoseEvent();
    }

    private void EndEvent()
    {
        if(_loseCoroutine != null) StopCoroutine(_loseCoroutine);
        _loseCoroutine = null;

        _robot.SetActive(true);
        
        _mainUI.StopEvent();
        SceneMusic.State = MusicState.Normal;
        _quizControl.EndQuiz();
    }

    public void WinEvent()
    {
        EndEvent();
        _docControl.StartProcess();
    }
    
    public async void LoseEvent()
    {
        EndEvent();
        _camRb.useGravity = true;
        _camRb.isKinematic = false;
        _blackScreen.gameObject.SetActive(true);
        await Task.Delay(3500);
        
        _camRb.isKinematic = true;
        _camRb.useGravity = false;
        _cameraManager.transform.localEulerAngles = new Vector3(0, 0, 0);
        _cameraManager.ResetCamera();
        _playerTF.position = _bedPos.position;
        StartCoroutine(_blackScreen.EndFade());
        _startButton.Enabled = true;
        
        _daysService.StartNewDay(DaysService.CurrentDay);
       
    }
    
    // [Header("EventSettings")] 
    // [SerializeField] private uint _puzzleTime;
    // [SerializeField] private uint _cost = 4;
    // [SerializeField] private Transform _player;
    // [SerializeField] private Vector3 _bedPos;
    // [Header("Puzzle")]
    // [SerializeField] private GameObject _wires;
    // [SerializeField] private GameObject _gears;
    // [SerializeField] private GameObject _meteorites;
    // [Header("Robots")] 
    // [SerializeField] private GameObject _robotPrefab;
    // [SerializeField] private List<Transform> _positions;
    // [SerializeField] private List<Transform> _wiresPos;
    // [SerializeField] private List<Transform> _gearsPos;
    // [SerializeField] private List<Transform> _meteoritesPos;
    // [Header("UI")] 
    // [SerializeField] private ScreenFade _blackSreen;
    //
    // private Random _rnd = new Random();
    // private Coroutine _autoLoseRoutine;
    // private GameObject _puzzleObj, _currentRobot;
    // private CameraManager _cameraMng => _player.GetComponentInChildren<CameraManager>();
    // private Rigidbody _camRb => _cameraMng.GetComponent<Rigidbody>();
    // private StartButton _button => FindFirstObjectByType<StartButton>();
    // // private NPCManager _npcMng => FindFirstObjectByType<NPCManager>();
    // private VisualEffects _fx => FindFirstObjectByType<VisualEffects>();
    //
    // public static event Action OnLose, OnDone;
    //
    // private void Start()
    // {
    //     // NPCManager.RandomEvent += OnEventStart;
    //     StartCoroutine(_fx.ChangeGamma());
    // }
    //
    // private void OnEventStart()
    // {
    //     StartCoroutine(_fx.ChangeGamma(true));
    //
    //     switch (_rnd.Next(3))
    //     {
    //         case 0:
    //             _wires.SetActive(true);
    //             _puzzleObj = _wires;
    //             // if (_rnd.Next(1, 101) <= (Math.Pow(7,2)))
    //                 // _currentRobot = Instantiate(_robotPrefab, _wiresPos[_rnd.Next(_wiresPos.Count)]);
    //
    //             SceneMusic.State = MusicState.Wires;
    //             break;
    //         case 1:
    //             _gears.SetActive(true);
    //             _puzzleObj = _gears;
    //             // if (_rnd.Next(1, 101) <= (Math.Pow(7,2)))
    //                 // _currentRobot = Instantiate(_robotPrefab, _gearsPos[_rnd.Next(_gearsPos.Count)]);
    //             
    //             SceneMusic.State = MusicState.Gears;
    //             break;
    //         case 2:
    //             _meteorites.SetActive(true);
    //             _puzzleObj = _meteorites;
    //             // if (_rnd.Next(1, 101) <= (Math.Pow(7,2)))
    //                 // _currentRobot = Instantiate(_robotPrefab, _meteoritesPos[_rnd.Next(_meteoritesPos.Count)]);
    //             
    //             SceneMusic.State = MusicState.Meteorites;
    //             break;
    //     }
    //
    //     // _currentRobot.GetComponent<Robot>().Positions = _positions;
    //     _autoLoseRoutine = StartCoroutine(AutoLose());
    // }
    //
    // public void OnEventEnd()
    // {
    //     StopCoroutine(_autoLoseRoutine);
    //     if(_currentRobot) Destroy(_currentRobot);
    //     _puzzleObj.SetActive(false);
    //     
    //     StartCoroutine(_fx.ChangeGamma());
    //     // _npcMng.SelectNPC(false);
    //     
    //     SceneMusic.State = MusicState.Normal;
    //     OnDone?.Invoke();
    // }
    //
    // private IEnumerator AutoLose()
    // {
    //     yield return new WaitForSeconds(_puzzleTime);
    //     OnLose?.Invoke();
    //     Lose();
    // }
    //
    // public async void Lose()
    // {
    //     if(_autoLoseRoutine != null) 
    //         StopCoroutine(_autoLoseRoutine);
    //     _camRb.useGravity = true;
    //     _camRb.isKinematic = false;
    //     _blackSreen.gameObject.SetActive(true);
    //     await Task.Delay(3500);
    //     
    //     if(_puzzleObj) _puzzleObj.SetActive(false);
    //     
    //     _camRb.isKinematic = true;
    //     _camRb.useGravity = false;
    //     _cameraMng.transform.localEulerAngles = new Vector3(0, 0, 0);
    //     _cameraMng.ResetCamera();
    //     _player.position = _bedPos;
    //     if(_currentRobot) 
    //         Destroy(_currentRobot);
    //     
    //     StartCoroutine(_fx.ChangeGamma());
    //     
    //     await Task.Delay(1000);
    //     SceneMusic.State = MusicState.Normal;
    //     _button.Enabled = true;
    //     StartCoroutine(_blackSreen.EndFade());
    //     
    //     
    // }
}
