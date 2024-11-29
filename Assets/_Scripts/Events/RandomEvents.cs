using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.UI;
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
    [SerializeField] private ScreenFade _redSreen;
    [SerializeField] private ScreenFade _blackSreen;
    
    private Random _rnd = new Random();
    private Coroutine _autoLoseRoutine;
    private GameObject _puzzleObj, _currentRobot;
    private CameraManager _cameraMng => _player.GetComponentInChildren<CameraManager>();
    private NPCManager _npcMng => FindFirstObjectByType<NPCManager>();

    public static event Action OnLose, OnDone;
    
    private void Start()
    {
        NPCManager.RandomEvent += OnEventStart;
    }

    private void OnEventStart()
    {
        _redSreen.gameObject.SetActive(true);
        switch (_rnd.Next(3))
        {
            case 0:
                _wires.SetActive(true);
                _puzzleObj = _wires;
                if (_rnd.Next(1, 101) < (SettingsUI.RobotsCount ^ 2))
                    _currentRobot = Instantiate(_robotPrefab, _wiresPos[_rnd.Next(_wiresPos.Count)]);
                break;
            case 1:
                _gears.SetActive(true);
                _puzzleObj = _gears;
                if (_rnd.Next(1, 101) < (SettingsUI.RobotsCount ^ 2))
                    _currentRobot = Instantiate(_robotPrefab, _gearsPos[_rnd.Next(_gearsPos.Count)]);
                break;
            case 2:
                _meteorites.SetActive(true);
                _puzzleObj = _meteorites;
                if (_rnd.Next(1, 101) < (SettingsUI.RobotsCount ^ 2))
                    _currentRobot = Instantiate(_robotPrefab, _meteoritesPos[_rnd.Next(_meteoritesPos.Count)]);
                break;
        }

        _autoLoseRoutine = StartCoroutine(AutoLose());
    }

    public void OnEventEnd()
    {
        StopCoroutine(_autoLoseRoutine);
        if(_currentRobot) Destroy(_currentRobot);
        _puzzleObj.SetActive(false);
        
        StartCoroutine(_redSreen.EndFade());
        _npcMng.SelectNPC(false);
        
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
        await Task.Delay(2000);
        _cameraMng.ResetCamera();
        _player.position = _bedPos;
        if(_currentRobot) 
            Destroy(_currentRobot);
        
        await Task.Delay(100);
        
        StartCoroutine(_blackSreen.EndFade());
        StartCoroutine(_redSreen.EndFade());
    }
}
