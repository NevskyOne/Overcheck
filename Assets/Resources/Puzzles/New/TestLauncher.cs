using UnityEngine;

public class TestLauncher : MonoBehaviour
{
    void Start()
    {
        MathQuiz quiz = FindObjectOfType<MathQuiz>();
        MathQuizData data = new MathQuizData
        {
            Count = 3,
            X = 10,
            Y = 15,
            Z = 30,
            IsAdvanced = true,
            MinSlag = 3,
            MaxSlag = 4,
            MinNumber = -100,
            MaxNumber = 100,
            Lives = 3
        };
        quiz.StartQuiz(data);
    }
}