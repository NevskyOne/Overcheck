using UnityEngine;

public class QuizControl : MonoBehaviour
{
    [Header("Quizes")]
    [SerializeField] private MathQuiz _mathQuiz;
    [SerializeField] private MorseQuiz _morseQuiz;
    [SerializeField] private ReactorQuiz _reactorQuiz;

    [Header("Data")]
    [SerializeField] private MathQuizData _mathData;
    [SerializeField] private MorseQuizData _morseData;
    [SerializeField] private ReactorQuizData _reactorData;

    void Start()
    {
        //_mathQuiz.StartQuiz(_mathData);
        //_morseQuiz.StartQuiz(_morseData);
        _reactorQuiz.StartQuiz(_reactorData);
    }
}