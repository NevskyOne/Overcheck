using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region Interfaces
public interface IQuiz
{
    void StartQuiz(QuizData data);
    void Lose();
    void Win();
}

[System.Serializable]
public class QuizData { }

[System.Serializable]
public class MathQuizData : QuizData
{
    public int Count;
    public int X;
    public int Y;
    public int Z;
    public bool IsAdvanced;
    public int MinSlag = 5;
    public int MaxSlag = 6;
    public int MinNumber = -50;
    public int MaxNumber = 50;
    public int Lives = 3;
}
#endregion

#region MathProblem Classes
public abstract class MathProblem
{
    public string ProblemText { get; protected set; }
    public int CorrectAnswer { get; protected set; }
    public int SlagCount { get; protected set; }
    public int MultCount { get; protected set; }
    public bool IsEquation { get; protected set; }

    public abstract bool CheckAnswer(int answer);
}

public class ExpressionProblem : MathProblem
{
    public ExpressionProblem(int minSlag, int maxSlag, int minNumber, int maxNumber)
    {
        Generate(minSlag, maxSlag, minNumber, maxNumber);
    }

    private void Generate(int minSlag, int maxSlag, int minNumber, int maxNumber)
    {
        if (Random.Range(0, 2) == 0)
        {
            int slagCount = Random.Range(minSlag, maxSlag + 1);
            SlagCount = slagCount;
            MultCount = 0;

            int result = Random.Range(minNumber, maxNumber + 1);
            ProblemText = result.ToString();

            for (int i = 1; i < slagCount; i++)
            {
                char op = Random.Range(0, 2) == 0 ? '+' : '-';
                int num = Random.Range(minNumber, maxNumber + 1);

                ProblemText += $" {op} {num}";
                result += op == '+' ? num : -num;

                if (result < minNumber || result > maxNumber)
                {
                    num = 0;
                    break;
                }
            }

            CorrectAnswer = result;
        }
        else
        {
            char op = '×';
            SlagCount = Random.Range(minSlag, maxSlag + 1);
            MultCount = SlagCount - 1;

            List<int> numbers = new List<int>();
            int result = 1;

            for (int i = 0; i < SlagCount; i++)
            {
                int num = Random.Range(minNumber, maxNumber + 1);
                while (num == 0) num = Random.Range(minNumber, maxNumber + 1);
                numbers.Add(num);
                result *= num;
            }

            ProblemText = string.Join(" × ", numbers);
            CorrectAnswer = result;
        }
    }

    public override bool CheckAnswer(int answer) => answer == CorrectAnswer;
}

public class EquationProblem : MathProblem
{
    private readonly int _minNumber;
    private readonly int _maxNumber;
    private readonly int _minSlag;
    private readonly int _maxSlag;

    public EquationProblem(int minNumber, int maxNumber, int minSlag, int maxSlag) : base()
    {
        _minNumber = minNumber;
        _maxNumber = maxNumber;
        _minSlag = minSlag;
        _maxSlag = maxSlag;

        bool valid = false;
        int attempts = 0;

        while (!valid && attempts < 1000)
        {
            GenerateComplexEquation();
            valid = CheckValidity();
            attempts++;

            Debug.Log($"Attempt {attempts}: {ProblemText} | Slag: {SlagCount} | Valid: {valid}");
        }

        if (!valid)
        {
            Debug.LogError("Не удалось сгенерировать уравнение!");
            GenerateFallback();
        }

        IsEquation = true;
    }

    private void GenerateComplexEquation()
    {
        int a = GetRandomNonZero();
        int b = Random.Range(1, 11);
        int operations = Random.Range(_minSlag - 2, _maxSlag - 1);
        string rightPart = GenerateExpression(operations);
        int rightValue = EvaluateExpression(rightPart);

        int x = b * rightValue + a;
        if (x < _minNumber || x > _maxNumber) return;

        ProblemText = $"(x - {a}) / {b} = {rightPart}";
        CorrectAnswer = x;
        SlagCount = 2 + operations + 1; // Левая часть (2) + операции справа + 1
        MultCount = 1;
    }

    private string GenerateExpression(int operations)
    {
        List<string> parts = new List<string>();
        int currentResult = Random.Range(_minNumber, _maxNumber + 1);
        parts.Add(currentResult.ToString());

        for (int i = 0; i < operations; i++)
        {
            char op = GetRandomOperator();
            int num = GetNumberForOperation(currentResult, op);

            parts.Add(op.ToString());
            parts.Add(num.ToString());

            currentResult = Calculate(currentResult, op, num);
        }

        return string.Join(" ", parts);
    }

    private int GetNumberForOperation(int current, char op)
    {
        switch (op)
        {
            case '/':
                int divisor = Random.Range(1, 11);
                while (current % divisor != 0)
                    divisor = Random.Range(1, 11);
                return divisor;
            case '×':
                return Random.Range(1, 11);
            default:
                return Random.Range(_minNumber, _maxNumber + 1);
        }
    }

    private int Calculate(int current, char op, int num)
    {
        switch (op)
        {
            case '+': return current + num;
            case '-': return current - num;
            case '×': return current * num;
            case '/': return current / num;
            default: return current;
        }
    }

    private int EvaluateExpression(string expression)
    {
        var elements = expression.Split(' ');
        int result = int.Parse(elements[0]);

        for (int i = 1; i < elements.Length; i += 2)
        {
            char op = elements[i][0];
            int num = int.Parse(elements[i + 1]);
            result = Calculate(result, op, num);
        }

        return result;
    }

    private bool CheckValidity()
    {
        // Проверка количества слагаемых
        if (SlagCount < _minSlag || SlagCount > _maxSlag)
            return false;

        // Проверка всех чисел в выражении
        var tokens = ProblemText.Split(new[] { ' ', '+', '-', '×', '/', '=', '(', ')' },
                                      System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            if (int.TryParse(token, out int num))
            {
                if (num < _minNumber || num > _maxNumber)
                    return false;
            }
        }

        return true;
    }

    private void GenerateFallback()
    {
        int a = GetRandomNonZero();
        int b = Random.Range(1, 11);
        int x = Random.Range(_minNumber, _maxNumber + 1);

        ProblemText = $"{a} × x = {a * x}";
        CorrectAnswer = x;
        SlagCount = 2;
        MultCount = 1;
    }

    private int GetRandomNonZero()
    {
        int num;
        do
        {
            num = Random.Range(_minNumber, _maxNumber + 1);
        } while (num == 0);
        return num;
    }

    private char GetRandomOperator()
    {
        char[] ops = { '+', '-', '×', '/' };
        return ops[Random.Range(0, ops.Length)];
    }

    public override bool CheckAnswer(int answer) => answer == CorrectAnswer;
}
#endregion

public class MathQuiz : MonoBehaviour, IQuiz
{
    [Header("UI Elements")]
    public TextMeshProUGUI problemText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;
    public InputField answerInput;
    public Button submitButton;
    public GameObject resultPanel;
    public TextMeshProUGUI resultMessage;

    private MathQuizData _currentData;
    private MathProblem _currentProblem;
    private float _timeRemaining;
    private int _solvedCount;
    private bool _isGameActive;
    private int _remainingLives;

    public void StartQuiz(QuizData data)
    {
        _currentData = data as MathQuizData;
        _remainingLives = _currentData.Lives;
        RestartQuiz();
    }

    public void RestartQuiz()
    {
        StopAllCoroutines();
        resultPanel.SetActive(false);
        _solvedCount = 0;
        _remainingLives = _currentData.Lives;
        _isGameActive = true;
        InitializeUI();
        GenerateNextProblem();
    }

    private void InitializeUI()
    {
        answerInput.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
        answerInput.text = "";
        livesText.text = $"{_remainingLives}";
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmitAnswer);
    }

    private void GenerateNextProblem()
    {
        if (_currentData.IsAdvanced)
        {
            _currentProblem = new EquationProblem(
                _currentData.MinNumber,
                _currentData.MaxNumber,
                _currentData.MinSlag,
                _currentData.MaxSlag);
        }
        else
        {
            _currentProblem = new ExpressionProblem(
                _currentData.MinSlag,
                _currentData.MaxSlag,
                _currentData.MinNumber,
                _currentData.MaxNumber);
        }

        _timeRemaining = _currentProblem.SlagCount * _currentData.X
            + _currentProblem.MultCount * _currentData.Y
            + (_currentProblem.IsEquation ? _currentData.Z : 0);

        problemText.text = _currentProblem.ProblemText;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (_timeRemaining > 0 && _isGameActive)
        {
            timerText.text = $"{_timeRemaining:F1}";
            yield return new WaitForSeconds(0.1f);
            _timeRemaining -= 0.1f;
        }

        if (_isGameActive)
        {
            timerText.text = "0";
            Lose();
        }
    }

    private void OnSubmitAnswer()
    {
        if (!_isGameActive) return;

        if (int.TryParse(answerInput.text, out int answer))
        {
            answerInput.text = "";

            if (_currentProblem.CheckAnswer(answer))
            {
                _solvedCount++;
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
                livesText.text = $"{_remainingLives}";
                if (_remainingLives <= 0)
                {
                    Lose();
                }
            }
        }
        else
        {
            answerInput.text = "";
        }
    }

    public void Lose()
    {
        ShowResult(false);
    }

    public void Win()
    {
        ShowResult(true);
    }

    private void ShowResult(bool isWin)
    {
        _isGameActive = false;

        resultPanel.SetActive(true);
        if (isWin)
        {
            resultMessage.text = "Победа! Все задачи решены!";
        }
        else
        {
            if (_timeRemaining <= 0)
            {
                resultMessage.text = "Поражение! Время вышло!";
            }
            else
            {
                resultMessage.text = $"Поражение! Ошибок больше, чем жизней ({_currentData.Lives})!";
            }
        }
    }
}