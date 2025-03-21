using System.Collections.Generic;
using UnityEngine;

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
    public ExpressionProblem(int minSlag, int maxSlag, int minNumber, int maxNumber, int minMult, int maxMult)
    {
        Generate(minSlag, maxSlag, minNumber, maxNumber, minMult, maxMult);
    }

    private void Generate(int minSlag, int maxSlag, int minNumber, int maxNumber, int minMult, int maxMult)
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

                string strNum = num < 0 ? $"({num})" : num.ToString();
                ProblemText += $" {op} {strNum}";
                result += op == '+' ? num : -num;

                if (result < minNumber || result > maxNumber)
                {
                    num = 0;
                    break;
                }
            }

            CorrectAnswer = result;
        }
        // В методе Generate класса ExpressionProblem
        else
        {
            SlagCount = Random.Range(minSlag, maxSlag + 1);
            MultCount = SlagCount - 1;
            List<string> parts = new List<string>();
            int result = 1;
            for (int i = 0; i < SlagCount; i++)
            {
                int num;
                char op;
                if (i == 0)
                {
                    num = GetNonZeroNumber(minMult, maxMult);
                    parts.Add(num.ToString());
                    result = num;
                }
                else
                {
                    op = Random.Range(0, 2) == 0 ? '×' : '/';
                    if (op == '/')
                    {
                        // Получаем делитель, который делит текущий результат без остатка
                        num = GetDivisor(result, minMult, maxMult);
                        if (num == 0) // Если делитель не найден, используем умножение
                        {
                            op = '×';
                            num = GetNonZeroNumber(minMult, maxMult);
                        }
                    }
                    else
                    {
                        num = GetNonZeroNumber(minMult, maxMult);
                    }

                    parts.Add($"{op} {num}");
                    result = (op == '×') ? result * num : result / num;
                }
            }
            ProblemText = string.Join(" ", parts);
            CorrectAnswer = result;
        }
    }

    private int GetNonZeroNumber(int min, int max)
    {
        int num;
        do
        {
            num = Random.Range(min, max + 1);
        } while (num == 0);
        return num;
    }

    private int GetDivisor(int value, int min, int max)
    {
        int attempts = 0;
        while (attempts < 100)
        {
            int divisor = Random.Range(min, max + 1);
            if (divisor != 0 && value % divisor == 0)
                return divisor;
            attempts++;
        }
        return 0; // Если не найден, вернуть 0 для смены операции
    }

    public override bool CheckAnswer(int answer) => answer == CorrectAnswer;
}

public class EquationProblem : MathProblem
{
    private readonly int _minNumber;
    private readonly int _maxNumber;
    private readonly int _minSlag;
    private readonly int _maxSlag;
    private readonly int _minMult;
    private readonly int _maxMult;

    public EquationProblem(int minSlag, int maxSlag, int minNumber, int maxNumber, int minMult, int maxMult)
    {
        // Проверка корректности настроек
        if (minNumber > maxNumber || minMult > maxMult || minSlag > maxSlag)
        {
            Debug.LogError("Неверные настройки диапазонов!");
            GenerateFallback();
            return;
        }

        _minNumber = minNumber;
        _maxNumber = maxNumber;
        _minSlag = minSlag;
        _maxSlag = maxSlag;
        _minMult = minMult;
        _maxMult = maxMult;

        bool valid = false;
        int attempts = 0;
        int maxAttempts = 100;

        while (!valid && attempts < maxAttempts)
        {
            GenerateComplexEquation();
            valid = CheckValidity();
            attempts++;
        }

        if (!valid)
        {
            GenerateFallback();
        }
    }


    private void GenerateComplexEquation()
    {
        int a = GetRandomNonZero();
        int b = GetRandomNonZero(); // Добавлена проверка на ноль

        // Ограничение количества операций
        int maxOperations = Mathf.Max(1, _maxSlag - 2);
        int operations = Random.Range(
            Mathf.Max(1, _minSlag - 2),
            maxOperations + 1
        );

        string[] rightPart = GenerateExpression(operations);
        int rightValue = EvaluateExpression(rightPart[0]);

        if (b == 0 || rightValue == 0)
        {
            GenerateFallback();
            return;
        }

        int x = a + b * rightValue;

        if (x < _minNumber || x > _maxNumber)
        {
            GenerateFallback();
            return;
        }

        string strA = a < 0 ? $"({a})" : a.ToString();
        ProblemText = $"(x - {strA}) / {b} = {rightPart[1]}";
        CorrectAnswer = x;
        SlagCount = 2 + operations + 1;
        MultCount = 1;
    }

    private string[] GenerateExpression(int operations)
    {
        List<string> parts = new List<string>();
        List<string> partsPrint = new List<string>();
        int currentResult = Random.Range(_minNumber, _maxNumber + 1);
        parts.Add(currentResult.ToString());
        partsPrint.Add(currentResult.ToString());

        for (int i = 0; i < operations; i++)
        {
            char op = GetRandomOperator();
            int num = GetNumberForOperation(currentResult, op);
            string strNum = num < 0 ? $"({num})" : num.ToString();

            parts.Add(op.ToString());
            parts.Add(num.ToString());

            partsPrint.Add(op.ToString());
            partsPrint.Add(strNum);

            currentResult = Calculate(currentResult, op, num);

            // Проверка выхода за границы
            if (currentResult < _minNumber || currentResult > _maxNumber)
            {
                GenerateFallback();
                return new[] { "", "" };
            }
        }

        return new[] { string.Join(" ", parts), string.Join(" ", partsPrint) };
    }

    private int GetNumberForOperation(int current, char op)
    {
        switch (op)
        {
            case '/':
                int divisor = Random.Range(_minMult, _maxMult + 1);
                while (current % divisor != 0)
                    divisor = Random.Range(_minMult, _maxMult + 1);
                return divisor;
            case '×':
                return Random.Range(_minMult, _maxMult + 1);
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
            else if (token.Contains("x"))
            {
                // Разрешить только одну переменную x
                if (!token.Equals("x"))
                    return false;
            }
        }
        return true;
    }

    private void GenerateFallback()
    {
        int a, b, x;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            a = GetRandomNonZero();
            b = GetRandomNonZero();
            x = Random.Range(_minNumber, _maxNumber + 1);
            attempts++;
        }
        while ((a * x < _minNumber || a * x > _maxNumber) && attempts < maxAttempts);

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