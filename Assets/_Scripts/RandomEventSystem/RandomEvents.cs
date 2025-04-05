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
    private Player _player;
    private CameraManager _cameraManager;
    private QuizControl _quizControl;
    private DaysService _daysService;
    private DocumentControlService _docControl;
    private Coroutine _loseCoroutine;

    [Inject]
    private void Initialize(MainUI mainUI, CameraManager camMan, Player player,
        QuizControl quizControl, EventBus eventBus, DaysService daysService, DocumentControlService docControl)
    {
        _mainUI = mainUI;
        _player = player;
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
            _robot.SetActive(true);
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
    
    public void LoseEvent()
    {
        EndEvent();
        _player.Die();
    }
}
