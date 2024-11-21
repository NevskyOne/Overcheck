using TMPro;
using UnityEngine;

public class TimeLines : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(1, 31)] private int _date;
    [SerializeField] [Range(1, 12)] private int _month;
    [Header("UI")]
    [SerializeField] private TMP_Text _dayCount;
    [SerializeField] private TMP_Text _monthCount;
    [SerializeField] private GameObject _sleepUI;
    [SerializeField] private TMP_Text _correctText;
    [SerializeField] private TMP_Text _wrongText;
    [SerializeField] private TMP_Text _additionalText;
    [SerializeField] private TMP_Text _totalText;
            

    public int WeekDate { get; set; }
    public int Date => _date;
    public int Month => _month;

    public uint CorrectNPC { get; set; }
    public uint WrongNPC { get; set; }
    public uint Additional { get; set; }

    private uint _voidCounter;
    private uint _eternityCounter;
    private uint _fatherCounter;
    
    private PlayerData _playerData => GetComponent<PlayerData>();
    private PlayerInteractions _playerInter => GetComponent<PlayerInteractions>();
    

    private void Start()
    {
        _dayCount.text = _date.ToString();
        _monthCount.text = _month.ToString();
    }

    public void ChangeTimeline(TimeLine _timeLine, bool add = true)
    {
        switch (_timeLine)
        {
            case TimeLine.Void: _voidCounter = add? _voidCounter + 1 : _voidCounter - 1;
                break;
            case TimeLine.Eternity: _eternityCounter = add? _eternityCounter + 1 : _eternityCounter - 1;
                break;
            case TimeLine.Father: _fatherCounter = add? _fatherCounter + 1 : _fatherCounter - 1;
                break;
        }
    }

    public void ChangeDay()
    {
        WeekDate += 1;
        _date += 1;
        if (_date >= 31)
        {
            _month += 1;
            _date = 1;
            _monthCount.text = _month.ToString();
        }
        _dayCount.text = _date.ToString();
    }
    
    public void Sleep()
    {
        _playerData.ChageCoins((int)(CorrectNPC + Additional));
        _playerData.ChageCoins((int)WrongNPC, false);
        _correctText.text = CorrectNPC.ToString();
        _wrongText.text = WrongNPC.ToString();
        _additionalText.text = Additional.ToString();
        _totalText.text = ((int)(CorrectNPC + Additional) - (int)WrongNPC).ToString();
        _sleepUI.SetActive(true);

        CorrectNPC = 0;
        Additional = 0;
        WrongNPC = 0;
        
        _playerInter.StopFocus();
    }
}

public enum TimeLine
{
    Void,
    Eternity,
    Father
}