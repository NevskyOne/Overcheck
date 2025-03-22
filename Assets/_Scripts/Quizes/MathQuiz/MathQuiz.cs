using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MathQuiz : MonoBehaviour, IQuiz
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _problemText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private InputField _answerInput;
    [SerializeField] private Button _submitButton;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _resultMessage;
    [SerializeField] private TextMeshProUGUI _progressText;

    private MathQuizData _currentData;
    private MathProblem _currentProblem;
    private float _timeRemaining;
    private int _solvedCount;
    private bool _isGameActive;
    private int _remainingLives;
    
    [Inject] private EventBus _eventBus;
    
    public void StartQuiz(IQuizData data)
    {
        _currentData = (MathQuizData)data;

        if (_currentData.MinNumber > _currentData.MaxNumber ||
            _currentData.MinMultiplaier > _currentData.MaxMultiplaier ||
            _currentData.MinSlag > _currentData.MaxSlag)
        {
            Debug.LogError("Неверные настройки квиза!");
            return;
        }

        _remainingLives = _currentData.Lives;
        RestartQuiz();
    }

    public void RestartQuiz()
    {
        StopAllCoroutines();
        _resultPanel.SetActive(false);
        _solvedCount = 0;
        _remainingLives = _currentData.Lives;
        _isGameActive = true;
        InitializeUI();
        GenerateNextProblem();
    }

    private void InitializeUI()
    {
        _answerInput.gameObject.SetActive(true);
        _submitButton.gameObject.SetActive(true);
        _answerInput.text = "";
        _livesText.text = $"{_remainingLives}";
        _submitButton.onClick.RemoveAllListeners();
        _submitButton.onClick.AddListener(OnSubmitAnswer);
        UpdateProgressDisplay();
    }

    private void UpdateProgressDisplay()
    {
        if (_progressText != null)
        {
            _progressText.text = $"Прогресс: {_solvedCount}/{_currentData.Count}";
        }
    }

    private void GenerateNextProblem()
    {
        if (_currentData.IsAdvanced)
        {
            _currentProblem = new EquationProblem(
                _currentData.MinSlag,
                _currentData.MaxSlag,
                _currentData.MinNumber,
                _currentData.MaxNumber,
                _currentData.MinMultiplaier,
                _currentData.MaxMultiplaier);
        }
        else
        {
            _currentProblem = new ExpressionProblem(
                _currentData.MinSlag,
                _currentData.MaxSlag,
                _currentData.MinNumber,
                _currentData.MaxNumber,
                _currentData.MinMultiplaier,
                _currentData.MaxMultiplaier);
        }

        _timeRemaining = _currentProblem.SlagCount * _currentData.X
            + _currentProblem.MultCount * _currentData.Y
            + (_currentProblem.IsEquation ? _currentData.Z : 0);

        _problemText.text = _currentProblem.ProblemText;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (_timeRemaining > 0 && _isGameActive)
        {
            _timerText.text = $"{_timeRemaining:F1}";
            yield return new WaitForSeconds(0.1f);
            _timeRemaining -= 0.1f;
        }

        if (_isGameActive)
        {
            _timerText.text = "0";
            Lose();
        }
    }

    private void OnSubmitAnswer()
    {
        if (!_isGameActive) return;

        if (int.TryParse(_answerInput.text, out int answer))
        {
            _answerInput.text = "";

            if (_currentProblem.CheckAnswer(answer))
            {
                _solvedCount++;
                UpdateProgressDisplay();
                if (_solvedCount >= _currentData.Count)
                {
                    Win();
                }
                else
                {
                    GenerateNextProblem();
                }
            }
            else
            {
                _remainingLives--;
                _livesText.text = $"{_remainingLives}";
                if (_remainingLives <= 0)
                {
                    Lose();
                }
            }
        }
        else
        {
            _answerInput.text = "";
        }
    }

    public void Lose()
    {
        ShowResult(false);
        _eventBus.Invoke(new LoseEvent());
    }

    public void Win()
    {
        ShowResult(true);
        _eventBus.Invoke(new WinEvent());
    }

    private void ShowResult(bool isWin)
    {
        _isGameActive = false;

        _resultPanel.SetActive(true);
        if (isWin)
        {
            _resultMessage.text = "Победа! Все задачи решены!";
        }
        else
        {
            if (_timeRemaining <= 0)
            {
                _resultMessage.text = "Поражение! Время вышло!";
            }
            else
            {
                _resultMessage.text = $"Поражение! Ошибок больше, чем жизней ({_currentData.Lives})!";
            }
        }
    }
}