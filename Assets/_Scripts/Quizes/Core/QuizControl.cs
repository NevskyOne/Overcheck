using System.Collections.Generic;
using UnityEngine;

public class QuizControl : MonoBehaviour
{
    [Header("Quizes")]
    [SerializeField] private List<GameObject> _quizes;

    [Header("Data")]
    [SerializeReference,SerializeReferenceButton] private List<IQuizData> _quizDatas;

    private int _quizIndex;
    
    public void StartQuiz(int index)
    {
        _quizIndex = index;
        _quizes[index].SetActive(true);
        _quizes[index].GetComponent<IQuiz>().StartQuiz(_quizDatas[index]);
    }

    public void EndQuiz()
    {
        _quizes[_quizIndex].SetActive(false);
    }
}