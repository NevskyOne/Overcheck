using UnityEngine;

public class QuizControl : MonoBehaviour
{
    [Header("Quizes")]
    [SerializeField] private MathQuiz _mathQuiz;
    [SerializeField] private MorseQuiz _morseQuiz;

    [Header("Data")]
    [SerializeField] private MathQuizData _mathData;
    [SerializeField] private MorseQuizData _morseData;

    void Start()
    {
        _mathQuiz.StartQuiz(_mathData);
        //_morseQuiz.StartQuiz(_morseData);
    }
}