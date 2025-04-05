using System.Collections.Generic;
using UnityEngine;

public class QuizControl : MonoBehaviour
{
    [Header("Quizes")] 
    [SerializeField] private List<GameObject> _quizes;
    [SerializeField] private List<GameObject> _marks;
    

    private int _quizIndex;
    
    public void StartQuiz(int index)
    {
        _quizIndex = index;
        _quizes[index].SetActive(true);
        _quizes[index].GetComponent<IQuiz>().StartQuiz();
        _marks[index].SetActive(true);
    }

    public void EndQuiz()
    {
        _quizes[_quizIndex].SetActive(false);
    }
}