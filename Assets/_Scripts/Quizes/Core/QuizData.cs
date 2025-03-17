public interface QuizData {}

[System.Serializable]
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
