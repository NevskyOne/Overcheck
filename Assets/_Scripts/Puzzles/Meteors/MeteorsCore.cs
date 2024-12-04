using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private List<Transform> _spawnpoints = new();
    [SerializeField] private Transform _stationTransform;
    [SerializeField] private int _maxStationHealth;

    [SerializeField] private GameObject _restartButton;
    
    private int _meteorsDestroyed;
    private int _meteorsToWinCount;
    private int _currentStationHealth;

    public override void StartPuzzle()
    {
        if (_isPuzzleSolved) return;
        
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
            var newMeteor = Instantiate(_meteorsPrefabs[Random.Range(0, _meteorsPrefabs.Count)], spawnpoint.position, Quaternion.identity, _meteorsParent);
            newMeteor.GetComponent<Meteor>().SetProperties(_stationTransform, _meteorsSpeed);
            newMeteor.GetComponent<Meteor>().OnDestroyMeteor += OnDestroyMeteor;
            newMeteor.GetComponent<Meteor>().OnKickStation += OnKickStation;
        }
    }

    private void OnDestroyMeteor()
    {
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
        foreach (Transform meteor in _meteorsParent)
        {
            Destroy(meteor.gameObject);
        }

        StopAllCoroutines();
        
        _meteorsDestroyed = 0;
        _currentStationHealth = _maxStationHealth;
        
        _restartButton.SetActive(true);
    }

    protected override void SolvePuzzle()
    {
        base.SolvePuzzle();
        
        StopAllCoroutines();
        
        foreach (Transform meteor in _meteorsParent)
        {
            Destroy(meteor.gameObject);
        }
        
        _restartButton.SetActive(true);
    }

    public void Miss()
    {
        _currentStationHealth--;
        
        if (_currentStationHealth <= 0)
            LosePuzzle();
    }
}