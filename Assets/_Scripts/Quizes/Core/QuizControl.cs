using UnityEngine;

public class QuizControl : MonoBehaviour
{
    [Header("Quizes")]
    [SerializeField] private MathQuiz _mathQuiz;
    [Header("Data")]
    [SerializeField] private MathQuizData _mathData;
    
    void Start()
    {
        _mathQuiz.StartQuiz(_mathData);
    }
}