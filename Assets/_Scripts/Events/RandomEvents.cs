using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = System.Random;
using UnityEngine;

public class RandomEvents : MonoBehaviour
{
    [Header("EventSettings")] 
    [SerializeField] private uint _puzzleTime;
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _bedPos;
    [Header("Puzzle")]
    [SerializeField] private GameObject _wires;
    [SerializeField] private GameObject _gears;
    [SerializeField] private GameObject _meteorites;
    [Header("Robots")] 
    [SerializeField] private GameObject _robotPrefab;
    [SerializeField] private List<Transform> _wiresPos;
    [SerializeField] private List<Transform> _gearsPos;
    [SerializeField] private List<Transform> _meteoritesPos;
    [Header("UI")] 
    [SerializeField] private ScreenFade _blackSreen;
    
    private Random _rnd = new Random();
    private Coroutine _autoLoseRoutine;
    private GameObject _puzzleObj, _currentRobot;
    private CameraManager _cameraMng => _player.GetComponentInChildren<CameraManager>();
    private PlayerInteractions _playerInter => FindFirstObjectByType<PlayerInteractions>();
    private NPCManager _npcMng => FindFirstObjectByType<NPCManager>();
    private Effects _fx => FindFirstObjectByType<Effects>();

    public static event Action OnLose, OnDone;
    
    private void Start()
    {
        NPCManager.RandomEvent += OnEventStart;
        StartCoroutine(_fx.ChangeGamma());
    }

    private void OnEventStart()
    {
        StartCoroutine(_fx.ChangeGamma(true));

        switch (_rnd.Next(3))
        {
            case 0:
                _wires.SetActive(true);
                _puzzleObj = _wires;
                if (_rnd.Next(1, 101) <= (Math.Pow(7,2)))
                    _currentRobot = Instantiate(_robotPrefab, _wiresPos[_rnd.Next(_wiresPos.Count)]);

                SceneMusic.State = MusicState.Wires;
                break;
            case 1:
                _gears.SetActive(true);
                _puzzleObj = _gears;
                if (_rnd.Next(1, 101) <= (Math.Pow(7,2)))
                    _currentRobot = Instantiate(_robotPrefab, _gearsPos[_rnd.Next(_gearsPos.Count)]);
                
                SceneMusic.State = MusicState.Gears;
                break;
            case 2:
                _meteorites.SetActive(true);
                _puzzleObj = _meteorites;
                if (_rnd.Next(1, 101) <= (Math.Pow(7,2)))
                    _currentRobot = Instantiate(_robotPrefab, _meteoritesPos[_rnd.Next(_meteoritesPos.Count)]);
                
                SceneMusic.State = MusicState.Meteorites;
                break;
        }

        _autoLoseRoutine = StartCoroutine(AutoLose());
    }

    public void OnEventEnd()
    {
        StopCoroutine(_autoLoseRoutine);
        if(_currentRobot) Destroy(_currentRobot);
        _puzzleObj.SetActive(false);
        
        StartCoroutine(_fx.ChangeGamma());
        _npcMng.SelectNPC(false);
        
        SceneMusic.State = MusicState.Normal;
        
        OnDone?.Invoke();
    }

    private IEnumerator AutoLose()
    {
        yield return new WaitForSeconds(_puzzleTime);
        OnLose?.Invoke();
        Lose();
    }

    public async void Lose()
    {
        if(_autoLoseRoutine != null) 
            StopCoroutine(_autoLoseRoutine);
        
        _blackSreen.gameObject.SetActive(true);
        await Task.Delay(3500);
        _puzzleObj.SetActive(false);
        _cameraMng.ResetCamera();
        _player.position = _bedPos;
        if(_currentRobot) 
            Destroy(_currentRobot);
        
        StartCoroutine(_fx.ChangeGamma());
        
        await Task.Delay(1000);
        SceneMusic.State = MusicState.Normal;
        _playerInter.ResetButton();
        StartCoroutine(_blackSreen.EndFade());
        
    }
}
