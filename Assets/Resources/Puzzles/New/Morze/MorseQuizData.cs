using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MorseQuizData : QuizData
{
    public List<NameIDPair> NameIDList; // База данных имен и ID
    public int Count;
    public float BaseTime;             // Базовое время на задание
    public float ExtraTime;            // Доп. время при смещении
    public bool UseShift;              // Использовать ли смещение
    public int MinShift;               // Минимальное смещение
    public int MaxShift;               // Максимальное смещение
    public int Lives;                  // Количество жизней
}

[System.Serializable]
public class NameIDPair
{
    public string Name; // Имя медведя
    public int ID;      // Соответствующий ID
}