
using System.Collections.Generic;

public interface IQuizData { }

[System.Serializable]
public struct MathQuizData : IQuizData
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

[System.Serializable]
public class MorseQuizData : IQuizData
{
    public List<NameIDPair> NameIDList; // ���� ������ ���� � ID
    public int Count;
    public float BaseTime;             // ������� ����� �� �������
    public float ExtraTime;            // ���. ����� ��� ��������
    public bool UseShift;              // ������������ �� ��������
    public int MinShift;               // ����������� ��������
    public int MaxShift;               // ������������ ��������
    public int Lives;                  // ���������� ������
}

[System.Serializable]
public class WiresQuizData : IQuizData
{
    
}
