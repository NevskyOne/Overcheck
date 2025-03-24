using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReactorQuizData : QuizData
{
    // Добавлены проверки диапазонов
    [Range(0, 1000)] public int safeTempMin;
    [Range(0, 1000)] public int safeTempMax;
    public int requiredReactivity;
    public int requiredCriticality;
    public float timeLimit;
    [Range(1, 5)] public int lives;
    public List<RodType> rodTypes;
}

[System.Serializable]
public class RodType
{
    public string name;
    public int reactivity;
    public int criticality;
    public int temperature;
    [SerializeField] public GameObject prefab;
}