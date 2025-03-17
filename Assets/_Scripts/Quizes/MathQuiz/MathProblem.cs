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
        else
        {
           
            SlagCount = Random.Range(minSlag, maxSlag + 1);
            MultCount = SlagCount - 1;

            List<int> numbers = new List<int>();
            int result = 1;

            for (int i = 0; i < SlagCount; i++)
            {
                char op = Random.Range(0, 2) == 0 ? '×' : '/';
                int num = Random.Range(minMult, maxMult + 1);
                while (num == 0) num = Random.Range(minMult, maxMult + 1);
                
                ProblemText = string.Join($" {op} ", numbers);
                numbers.Add(num);
                if (op == '×') 
                    result *= num;
                else
                    result /= num;
            }

            
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
    private readonly int _minMult;
    private readonly int _maxMult;

    public EquationProblem(int minSlag, int maxSlag, int minNumber, int maxNumber, int minMult, int maxMult)
    {
        _minNumber = minNumber;
        _maxNumber = maxNumber;
        _minSlag = minSlag;
        _maxSlag = maxSlag;
        _minMult = minMult;
        _maxMult = maxMult;

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
        int b = Random.Range(_minMult, _maxMult + 1);
        int operations = Random.Range(_minSlag - 2, _maxSlag - 1);
        string[] rightPart = GenerateExpression(operations);
        int rightValue = EvaluateExpression(rightPart[0]);

        int x = b * rightValue + a;
        if (x < _minNumber || x > _maxNumber) return;
        
        string strA = a < 0 ? $"({a})" : a.ToString();
        
        ProblemText = $"(x - {strA}) / {b} = {rightPart[1]}";
        CorrectAnswer = x;
        SlagCount = 2 + operations + 1; // Левая часть (2) + операции справа + 1
        MultCount = 1;
    }

    private string[] GenerateExpression(int operations)
    {
        List<string> parts = new List<string>();
        List<string> partsPrint = new List<string>();
        int currentResult = Random.Range(_minNumber, _maxNumber + 1);
        parts.Add(currentResult.ToString());

        for (int i = 0; i < operations; i++)
        {
            char op = GetRandomOperator();
            int num = GetNumberForOperation(currentResult, op);
            
            string strNum = num < 0 ? $"({num})" : num.ToString();
            partsPrint.Add(strNum);
            
            parts.Add(op.ToString());
            parts.Add(num.ToString());

            currentResult = Calculate(currentResult, op, num);
        }

        return new []{string.Join(" ", parts), string.Join(" ", partsPrint)};
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
        }

        return true;
    }

    private void GenerateFallback()
    {
        int a = GetRandomNonZero();
        int b = Random.Range(_minNumber, _maxNumber);
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