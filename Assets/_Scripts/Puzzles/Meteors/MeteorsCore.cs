using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorsCore : Puzzle
{
    [Header("Meteors options")]
    [SerializeField] private List<GameObject> _meteorsPrefabs = new();
    [SerializeField] private Transform _meteorsParent;
    [SerializeField] private float _meteorsSpeed;
    [SerializeField] private float _meteorsSpawnRateMin;
    [SerializeField] private float _meteorsSpawnRateMax;
    [SerializeField] private int _meteorsToWinCountMin;
    [SerializeField] private int _meteorsToWinCountMax;

    [Header("Enviroment")] 
    [SerializeField] private GameObject _startButton;
    [SerializeField] private List<Transform> _spawnpoints = new();
    [SerializeField] private Transform _stationTransform;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private int _maxStationHealth;
    [SerializeField] private float _zPos = 0.985f;
    
    
    private int _meteorsDestroyed;
    private int _meteorsToWinCount;
    private int _currentStationHealth;

    private bool _isActive;
    
    private void Update()
    {
        if(_isActive)
            _targetTransform.localPosition = GetMouseWorldPosition();
    }

    public override void StartPuzzle()
    {
        _startButton.SetActive(true);
        _isActive = false;
    }
    public void ActivatePuzzle()
    {
        if (_isPuzzleSolved) return;
        _isActive = true;
        _meteorsDestroyed = 0;
        _currentStationHealth = _maxStationHealth;
        _meteorsToWinCount = Random.Range(_meteorsToWinCountMin, _meteorsToWinCountMax);
        
        StartCoroutine(SpawnMeteors());
    }

    private IEnumerator SpawnMeteors()
    {
        while (!_isPuzzleSolved)
        {
            yield return new WaitForSeconds(Random.Range(_meteorsSpawnRateMin, _meteorsSpawnRateMax));
            var spawnpoint = _spawnpoints[Random.Range(0, _spawnpoints.Count)];
            var newMeteor = Instantiate(_meteorsPrefabs[Random.Range(0, _meteorsPrefabs.Count)], spawnpoint.position, Quaternion.Euler(0,0,0), _meteorsParent).GetComponent<Meteor>();
            newMeteor.transform.localRotation = Quaternion.Euler(0, 0, 0);
            newMeteor.transform.localPosition = spawnpoint.localPosition;
            newMeteor.SetProperties(_stationTransform, _meteorsSpeed);
            newMeteor.OnDestroyMeteor += OnDestroyMeteor;
            newMeteor.OnKickStation += OnKickStation;
        }
    }

    private void OnDestroyMeteor()
    {
        _targetTransform.GetChild(0).gameObject.SetActive(true);
        _meteorsDestroyed++;
        if (_meteorsDestroyed >= _meteorsToWinCount)
            SolvePuzzle();
    }

    private void OnKickStation(GameObject meteor)
    {
        _currentStationHealth--;
        
        if (_currentStationHealth <= 0)
            LosePuzzle();
    }

    protected override void LosePuzzle()
    {
        base.LosePuzzle();
        foreach (Transform meteor in _meteorsParent)
        {
            Destroy(meteor.gameObject);
        }

        StopAllCoroutines();
        
        _meteorsDestroyed = 0;
        _currentStationHealth = _maxStationHealth;
    }

    protected override void SolvePuzzle()
    {
        base.SolvePuzzle();
        
        StopAllCoroutines();
        
        foreach (Transform meteor in _meteorsParent)
        {
            Destroy(meteor.gameObject);
        }
    }

    public void Miss()
    {
        if(!_isActive) return;
        
        _targetTransform.GetChild(0).gameObject.SetActive(true);
        _currentStationHealth--;
        
        if (_currentStationHealth <= 0)
            LosePuzzle();
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = _zPos;
        
        var worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        var localPos = transform.InverseTransformPoint(worldPos);
        return new Vector3(localPos.x, localPos.y, 0);
    }
}