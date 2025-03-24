using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ReactorQuiz : MonoBehaviour, IQuiz
{
    [Header("Settings")]
    [SerializeField] private ReactorRodSlot[] _slots;
    [SerializeField] private Transform _inventoryParent;
    [SerializeField] private ReactorRod _rodPrefab;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text _temperatureText;
    [SerializeField] private TMP_Text _reactivityText;
    [SerializeField] private TMP_Text _criticalityText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _resultText;

    private ReactorQuizData _data;
    private List<ReactorRod> _activeRods = new List<ReactorRod>();
    private Stack<ReactorRod> _rodPool = new Stack<ReactorRod>();
    private int _currentLives;
    private float _currentTime;
    private bool _isActive;
    private Coroutine _timerCoroutine;

    public void StartQuiz(QuizData data)
    {
        if (data is not ReactorQuizData reactorData)
        {
            Debug.LogError("Invalid quiz data type!");
            return;
        }

        _data = reactorData;
        InitializeGame();
    }

    private void InitializeGame()
    {
        Cleanup();
        InitializeRods();

        _currentLives = _data.lives;
        _currentTime = _data.timeLimit;
        _isActive = true;

        UpdateUI();
        _timerCoroutine = StartCoroutine(GameTimer());
    }

    private void InitializeRods()
    {
        foreach (var rodType in _data.rodTypes)
        {
            for (int i = 0; i < 3; i++)
            {
                var rod = CreateRod(rodType);
                rod.gameObject.SetActive(false);
                _rodPool.Push(rod);
            }
        }
    }

    private ReactorRod CreateRod(RodType rodType)
    {
        var rod = Instantiate(_rodPrefab, _inventoryParent);
        rod.Initialize(rodType);
        return rod;
    }

    private IEnumerator GameTimer()
    {
        while (_currentTime > 0 && _isActive)
        {
            _currentTime -= Time.deltaTime;
            _timerText.text = $"Time: {Mathf.Max(_currentTime, 0):F1}";
            yield return null;
        }

        if (_isActive) HandleGameOver(false);
    }

    public void OnRodPlaced(int slotIndex, ReactorRod rod)
    {
        _activeRods.Add(rod);
        CheckSolution();
    }

    private void CheckSolution()
    {
        int totalTemp = 0;
        int totalReactivity = 0;
        int totalCriticality = 0;

        foreach (var rod in _activeRods)
        {
            totalTemp += rod.RodType.temperature;
            totalReactivity += rod.RodType.reactivity;
            totalCriticality += rod.RodType.criticality;
        }

        UpdateUI(totalTemp, totalReactivity, totalCriticality);
        CheckWinConditions(totalTemp, totalReactivity, totalCriticality);
    }

    private void CheckWinConditions(int temp, int reactivity, int criticality)
    {
        bool tempValid = temp >= _data.safeTempMin && temp <= _data.safeTempMax;
        bool reactivityValid = reactivity == _data.requiredReactivity;
        bool criticalityValid = criticality >= _data.requiredCriticality;

        if (tempValid && reactivityValid && criticalityValid)
        {
            HandleGameOver(true);
        }
        else if (!_isActive)
        {
            _currentLives--;
            UpdateUI();

            if (_currentLives <= 0)
            {
                HandleGameOver(false);
            }
        }
    }

    private void UpdateUI(int temp = 0, int reactivity = 0, int criticality = 0)
    {
        _temperatureText.text = $"Temperature: {temp} [{_data.safeTempMin}-{_data.safeTempMax}]";
        _reactivityText.text = $"Reactivity: {reactivity} (Target: {_data.requiredReactivity})";
        _criticalityText.text = $"Criticality: {criticality} (Min: {_data.requiredCriticality})";
        _livesText.text = $"Attempts: {_currentLives}";
    }

    private void HandleGameOver(bool isWin)
    {
        _isActive = false;
        StopCoroutine(_timerCoroutine);

        _resultPanel.SetActive(true);
        _resultText.text = isWin ? "Reactor stabilized!" : "Critical failure!";
        _resultText.color = isWin ? Color.green : Color.red;
    }

    public void ReturnRodToInventory(ReactorRod rod)
    {
        rod.ResetRod();
        _activeRods.Remove(rod);
        _rodPool.Push(rod);
        CheckSolution();
    }

    private void Cleanup()
    {
        foreach (var rod in _activeRods)
        {
            Destroy(rod.gameObject);
        }
        _activeRods.Clear();

        foreach (var rod in _rodPool)
        {
            Destroy(rod.gameObject);
        }
        _rodPool.Clear();
    }

    public void Win() => HandleGameOver(true);

    public void Lose() => HandleGameOver(false);
}