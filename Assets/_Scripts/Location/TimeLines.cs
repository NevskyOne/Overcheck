using System;
using System.Threading.Tasks;
using _Scripts.UI;
using TMPro;
using UnityEngine;

public class TimeLines : MonoBehaviour
{
    [Header("Settings")] [SerializeField] [Range(1, 7)]
    private int _days;

    [SerializeField] [Range(1, 31)] private int _date;
    [SerializeField] [Range(1, 12)] private int _month;
    [Header("Days")] 
    [SerializeField] private TMP_Text _dayCount;
    [SerializeField] private TMP_Text _monthCount;
    [SerializeField] private TMP_Text _laptopTime;
    [Header("Sleep")] 
    [SerializeField] private GameObject _sleepUI;
    [SerializeField] private TMP_Text _correctText;
    [SerializeField] private TMP_Text _wrongText;
    [SerializeField] private TMP_Text _additionalText;
    [SerializeField] private TMP_Text _totalText;
    [Header("Title")] 
    [SerializeField] private GameObject _titleUI;
    [SerializeField] private TMP_Text _endText;

    public int WeekDate { get; private set; }
    public static float MoneyFactor { get; set; } = 1;
    public int Date => _date;
    public int Month => _month;

    public uint CorrectNPC { get; set; }
    public uint WrongNPC { get; set; }
    public uint Additional { get; set; }

    private uint _voidCounter,_eternityCounter,_robotsCounter;
    private uint _voidTemp, _eternityTemp, _robotsTemp;
    
    private DataBase _dataBase => FindFirstObjectByType<DataBase>();

    public static event Action OnDayEnd;

    private void Start()
    {
        WeekDate = SettingsUI.CurrentDay;
        _date += WeekDate;
        _dayCount.text = $"День:{_date}";
        _monthCount.text = $"Месяц:{_month}";
        _laptopTime.text = $"{_date}/{_month}";
        
        RandomEvents.OnLose += ResetDay;
        OnDayEnd?.Invoke();
    }

    public void ChangeTimeline(TimeLine timeLine, bool add = true)
    {
        switch (timeLine)
        {
            case TimeLine.Void:
                _voidTemp = add ? _voidTemp + 1 : _voidTemp - 1;
                break;
            case TimeLine.Eternity:
                _eternityTemp = add ? _eternityTemp + 1 : _eternityTemp - 1;
                break;
        }
    }

    public void ChangeDay()
    {
        WeekDate += 1;
        SettingsUI.CurrentDay += 1;

        _date += 1;
        if (_date >= 31)
        {
            _month += 1;
            _date = 1;
            _monthCount.text = _month.ToString();
        }

        _dayCount.text = $"День:{_date}";
        _monthCount.text = $"Месяц:{_month}";
        _laptopTime.text = $"{_date}/{_month}/2144";

        _dataBase.ClearData();

        OnDayEnd?.Invoke();
        
    }

    public void ResetDay()
    {
        _robotsTemp = 0;
        _eternityTemp = 0;
        _voidTemp = 0;
        CorrectNPC = 0;
        WrongNPC = 0;
        Additional = 0;
    }
    
    public async void Sleep()
    {
        CorrectNPC = (uint)(CorrectNPC * MoneyFactor);
        Additional = (uint)(Additional * MoneyFactor);
        PlayerData.ChangeCoins((int)(CorrectNPC + Additional));
        PlayerData.ChangeCoins((int)WrongNPC, false);

        _robotsCounter = _robotsTemp;
        _eternityCounter = _eternityTemp;
        _voidCounter = _voidTemp;
        if (WeekDate == _days - 1)
        {
            if (_eternityCounter > 4)
            {
                _endText.text = "Этернити";
                SettingsUI.Eternity = true;
            }
            else if (_voidCounter > 20)
            {
                _endText.text = "Хорошая Void";
                SettingsUI.GoodVoid = true;
            }
            else
            {
                _endText.text = "Плохая Void";
                SettingsUI.BadVoid = true;
            }

            SettingsUI.CurrentDay = 0;
            _titleUI.SetActive(true);
        }
        else
        {
            _correctText.text = $"{CorrectNPC}";
            _wrongText.text = $"{WrongNPC}";
            _additionalText.text = $"{Additional}";
            _totalText.text = $"{PlayerData.HoneyCombs}";
            _sleepUI.SetActive(true);

            CorrectNPC = 0;
            Additional = 0;
            WrongNPC = 0;
        }
    }
}

public enum TimeLine
{
    Void,
    Eternity,
    Robots,
    Tutorial
}