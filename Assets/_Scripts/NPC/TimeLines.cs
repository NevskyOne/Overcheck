using TMPro;
using UnityEngine;

public class TimeLines : MonoBehaviour
{
    [SerializeField] [Range(1, 31)] private int _date;
    [SerializeField] [Range(1, 12)] private int _month;

    [SerializeField] private TMP_Text _dayCount;
    [SerializeField] private TMP_Text _monthCount;

    public int Date => _date;
    public int Month => _month;

    private void Start()
    {
        _dayCount.text = _date.ToString();
        _monthCount.text = _month.ToString();
    }

    public void ChangeDay()
    {
        _date += 1;
        if (_date >= 31)
        {
            _month += 1;
            _date = 1;
            _monthCount.text = _month.ToString();
        }
        _dayCount.text = _date.ToString();
    }
}

public enum TimeLine
{
    Void,
    Eternity,
    Scary
}