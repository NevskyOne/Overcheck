using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class NPC : MonoBehaviour
{
    [SerializeField] private Mesh[] _models;
    
    [Header("Docs")]
    [SerializeField] private Sprite _photo;
    [SerializeField] private Goal _targetGoal;
    [SerializeField] private GameObject _PMS;
    [SerializeField] private GameObject _IIC;
    [SerializeField] private GameObject _PP;
    
    [Header("Valuable")]
    [SerializeField] private uint _cost;
    [SerializeField] private TimeLine _timeLine;
    
    [Header("Text")]
    [SerializeField] private List<DialogFragment> _fragments = new();
    [SerializeField] private DialogFragment _endPassText, _endBackText;
    
    private string _name;
    private int _planet;
    
    
    private Random _rnd = new Random();
    private bool _origin;
    private TimeLines _timeLines;
    private DialogSystem _dialogSys;
    private NPCManager _npcManager;
    
    private void Start()
    {
        _timeLines = FindFirstObjectByType<TimeLines>();
        _dialogSys = FindFirstObjectByType<DialogSystem>();
        _npcManager = FindFirstObjectByType<NPCManager>();
        GetComponent<MeshFilter>().mesh = _models[_rnd.Next(0, 3)];
        
        _name = RandomParamSt.Names[_rnd.Next(0,RandomParamSt.Names.Length)];
        _photo = RandomParamSt.Photos[_rnd.Next(0,RandomParamSt.Photos.Length)];
        _planet = _rnd.Next(1,5);
    }

    public void GiveDocs()
    {
        Document pp = null, iic = null;
        var pms = _npcManager.GiveDoc(_PMS, 1);
        pms.Initialize(_name, _photo, _planet);
        if (_timeLines.WeekDate > 2)
        {
            iic = _npcManager.GiveDoc(_IIC, 1);
            iic.Initialize(_name, _photo, _planet);
        }

        if (_timeLines.WeekDate > 4)
        {
            pp = _npcManager.GiveDoc(_PP, 1);
            pp.Initialize(_name, _photo, _planet);
        }

        switch (_timeLine)
        {
            case TimeLine.Void:
                _origin = Convert.ToBoolean(_rnd.Next(0, 1));
                if (!_origin)
                {
                    if (_timeLines.WeekDate > 2)
                        iic.Randomize(1);
                    if (_timeLines.WeekDate > 4)
                        pp.Randomize(1);
                    pms.Randomize(1);
                }
                break;
            case TimeLine.Eternity:
                _origin = false;
                if (_timeLines.WeekDate > 2)
                    iic.Randomize(1);
                if (_timeLines.WeekDate > 4)
                    pp.Randomize(1);
                pms.Randomize(1);
                break;
            case TimeLine.Scary:
                _origin = true;
                break;
        }
    }

    public void CheckCoast(Goal playerGoal, bool playerOrigin)
    {
        if ((_origin && playerOrigin && playerGoal == _targetGoal) || (_origin == false && playerOrigin == false))
        {
            _timeLines.CorrectNPC += _cost;
        }
        else if (_origin && playerOrigin)
        {
            _timeLines.CorrectNPC += (uint)(_cost * 0.75f);
        }
        else
        {
            _timeLines.WrongNPC += _cost;
        }
    }

    public void StartChat()
    {
        _dialogSys.FragmentsStack = _fragments.ToList();
        _dialogSys.PlayNext();
    }
    
}


public enum Planet
{
    Planet1,
    Planet2,
    Planet3,
    Planet4,
    Planet5
}

public enum Goal
{
    Work,
    Family,
    Vacation,
    Other
}
