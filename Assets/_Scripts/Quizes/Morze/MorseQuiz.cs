using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;
using Zenject;

public class MorseQuiz : MonoBehaviour, IQuiz
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _morseText;
    [SerializeField] private TextMeshProUGUI _shiftInfoText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TMP_InputField _answerInput;
    [SerializeField] private Button _submitButton;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _resultMessage;
    [SerializeField] private int _correctAnswersCount;
    [SerializeField] private int _requiredCorrect;
    [SerializeField] private TextMeshProUGUI _progressText;

    private MorseQuizData _currentData;
    private NameIDPair _currentPair;
    private float _timeRemaining;
    private int _remainingLives;
    private bool _isGameActive;
    private int _currentShift;
    
    [Inject] private EventBus _eventBus;

    private Dictionary<char, string> _morseCodeMap = new Dictionary<char, string>()
    {
        {'�', "�-"}, {'�', "-���"}, {'�', "�--"}, {'�', "--�"}, {'�', "-��"},
        {'�', "�"}, {'�', "�"}, {'�', "���-"}, {'�', "--��"}, {'�', "��"},
        {'�', "�---"}, {'�', "-�-"}, {'�', "�-��"}, {'�', "--"}, {'�', "-�"},
        {'�', "---"}, {'�', "�--�"}, {'�', "�-�"}, {'�', "���"}, {'�', "-"},
        {'�', "��-"}, {'�', "��-�"}, {'�', "����"}, {'�', "-�-�"}, {'�', "---�"},
        {'�', "----"}, {'�', "--.-"}, {'�', "--.--"}, {'�', "-.--"}, {'�', "-..-"},
        {'�', "��-��"}, {'�', "��--"}, {'�', "�-�-"}, {' ', "  "}
    };

    public void StartQuiz(IQuizData data)
    {
        _currentData = data as MorseQuizData;
        _requiredCorrect = _currentData.Count;
        ResetQuiz();
    }

    public void ResetQuiz()
    {
        StopAllCoroutines();
        _correctAnswersCount = 0;
        _remainingLives = _currentData.Lives;
        _isGameActive = true;
        UpdateProgress();
        InitializeUI();
        GenerateNewPuzzle();
    }

    private void RestartQuiz()
    {
        StopAllCoroutines();
        _resultPanel.SetActive(false);
        _isGameActive = true;
        InitializeUI();
        GenerateNewPuzzle();
    }

    private void InitializeUI()
    {
        _answerInput.gameObject.SetActive(true);
        _submitButton.gameObject.SetActive(true);
        _answerInput.text = "";
        _livesText.text = $"{_remainingLives}";
        _submitButton.onClick.RemoveAllListeners();
        _submitButton.onClick.AddListener(OnSubmitAnswer);
    }

    private void UpdateProgress()
    {
        _progressText.text = $"��������: {_correctAnswersCount}/{_requiredCorrect}";
    }

    private void GenerateNewPuzzle()
    {
        // ����� ���������� ����� �� ����
        _currentPair = _currentData.NameIDList[Random.Range(0, _currentData.NameIDList.Count)];

        // ���������� ��������
        string shiftedName = _currentPair.Name;
        _currentShift = 0;

        if (_currentData.UseShift)
        {
            _currentShift = Random.Range(_currentData.MinShift, _currentData.MaxShift + 1);
            shiftedName = ApplyShift(_currentPair.Name, _currentShift);
        }

        // ��������� Morse-����
        _morseText.text = ConvertToMorse(shiftedName);
        _shiftInfoText.text = _currentData.UseShift ? $"(�������� {_currentShift})" : "";

        // ��������� �������
        _timeRemaining = _currentData.BaseTime + (_currentData.UseShift ? _currentData.ExtraTime : 0);
        StartCoroutine(UpdateTimer());
    }

    private string ApplyShift(string input, int shift)
    {
        string alphabet = "�����Ũ��������������������������";
        char[] chars = input.ToUpper().ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ' ') continue;

            int index = alphabet.IndexOf(chars[i]);
            if (index == -1) continue;

            int newIndex = (index + shift) % alphabet.Length;
            if (newIndex < 0) newIndex += alphabet.Length;

            chars[i] = alphabet[newIndex];
        }
        return new string(chars);
    }

    private string ConvertToMorse(string name)
    {
        StringBuilder morse = new StringBuilder();
        foreach (char c in name.ToUpper())
        {
            if (_morseCodeMap.TryGetValue(c, out string code))
            {
                morse.Append(code);
                morse.Append(' ');
            }
        }
        return morse.ToString().Trim();
    }

    private IEnumerator UpdateTimer()
    {
        while (_timeRemaining > 0 && _isGameActive)
        {
            _timerText.text = $"{_timeRemaining:F1}";
            yield return new WaitForSeconds(0.1f);
            _timeRemaining -= 0.1f;
        }

        if (_isGameActive) Lose();
    }

    private void OnSubmitAnswer()
    {
        if (!_isGameActive || !int.TryParse(_answerInput.text, out int answer)) return;

        if (answer == _currentPair.ID)
        {
            _correctAnswersCount++;
            UpdateProgress();

            if (_correctAnswersCount >= _requiredCorrect)
            {
                Win();
            }
            else
            {
                GenerateNewPuzzle();
                _answerInput.text = "";
            }
        }
        else
        {
            _remainingLives--;
            _livesText.text = $"{_remainingLives}";
            if (_remainingLives <= 0) Lose();
        }
        _answerInput.text = "";
    }

    public void Lose()
    {
        _isGameActive = false;
        ShowResult("���������! ����� ����� ��� �������� ID!");
        _eventBus.Invoke(new LoseEvent());
        
    }

    public void Win()
    {
        _isGameActive = false;
        ShowResult("������! �� ������� ��� ID!");
        _eventBus.Invoke(new WinEvent());
    }

    private void ShowResult(string message)
    {
        _resultPanel.SetActive(true);
        _resultMessage.text = message;
    }
}