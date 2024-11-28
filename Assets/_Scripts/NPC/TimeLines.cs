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
    public int Date => _date;
    public int Month => _month;

    public uint CorrectNPC { get; set; }
    public uint WrongNPC { get; set; }
    public uint Additional { get; set; }

    private uint _voidCounter;
    private uint _eternityCounter;
    private uint _fatherCounter;

    private PlayerData _playerData => GetComponent<PlayerData>();
    private DataBase _dataBase => FindFirstObjectByType<DataBase>();

    public event Action OnDayEnd;

    private void Start()
    {
        WeekDate = SettingsUI.CurrentDay;
        _date += WeekDate;
        _dayCount.text = $"День:{_date}";
        _monthCount.text = $"Месяц:{_month}";
        _laptopTime.text = $"{_date}/{_month}/2144";
    }

    public void ChangeTimeline(TimeLine timeLine, bool add = true)
    {
        switch (timeLine)
        {
            case TimeLine.Void:
                _voidCounter = add ? _voidCounter + 1 : _voidCounter - 1;
                break;
            case TimeLine.Eternity:
                _eternityCounter = add ? _eternityCounter + 1 : _eternityCounter - 1;
                break;
            case TimeLine.Father:
                _fatherCounter = add ? _fatherCounter + 1 : _fatherCounter - 1;
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

    public async void Sleep()
    {
        _playerData.ChageCoins((int)(CorrectNPC + Additional));
        _playerData.ChageCoins((int)WrongNPC, false);
        if (WeekDate == _days - 1)
        {
            if (_fatherCounter > 4)
            {
                _endText.text = "Роботы";
                SettingsUI.Robots = true;
            }
            else if (_eternityCounter > 4)
            {
                _endText.text = "Вечность";
                SettingsUI.Eternity = true;
            }
            else if (_voidCounter > 4)
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
            await Task.Delay(2000);
            LoadManager.LoadScene(0);
        }
        else
        {
            _correctText.text = $"Правильные:{CorrectNPC}";
            _wrongText.text = $"Неверные:{WrongNPC}";
            _additionalText.text = $"Дополнительные:{Additional}";
            _totalText.text = $"Результат:{(int)(CorrectNPC + Additional) - (int)WrongNPC}";
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
    Father
}