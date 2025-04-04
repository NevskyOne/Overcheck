using System;

[Serializable]
public class QuizData { }

[Serializable]
public class MathQuizData : QuizData
{
    public int Count;
    public int X;
    public int Y;
    public int Z;
    public bool IsAdvanced;
    public int MinSlag;
    public int MaxSlag;
    public int MinNumber;
    public int MaxNumber;
    public int MinMultiplaier;
    public int MaxMultiplaier;
    public int Lives;
}

[Serializable]
public class MorseQuizData : QuizData
{ 
    public int Count;
    public float BaseTime;             
    public float ExtraTime;            
    public bool UseShift;              
    public int MinShift;               
    public int MaxShift;               
    public int Lives;                  
}

public class WiresQuizData : QuizData
{
    
}
