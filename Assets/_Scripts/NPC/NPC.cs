using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class NPC : MonoBehaviour
{
    public Goal CurrentGoal { get; set; } = Goal.Other;
    
    [SerializeField] private Sprite _photo;
    [SerializeField] private Goal _targetGoal;
    [SerializeField] private Mesh[] _models;
    [SerializeField] private uint _cost;
    [SerializeField] private TimeLine _timeLine;
    [SerializeField] private List<DialogFragment> _fragments = new();
    [SerializeField] private DialogFragment _endPassText, _endBackText;
    
    private string _name;
    private int _planet;
    
    private DialogSystem _dialogSys;
    private Random _rnd = new Random();
    private bool _origin;
    private TimeLines _timeLines;
    
    private void Start()
    {
        _timeLines = FindFirstObjectByType<TimeLines>();
        _dialogSys = FindFirstObjectByType<DialogSystem>();
        GetComponent<MeshFilter>().mesh = _models[_rnd.Next(0, 3)];
        
        _name = RandomParamSt.Names[_rnd.Next(0,RandomParamSt.Names.Length)];
        _planet = _rnd.Next(1,5);
        
        var iic = new IICDocument(_name, _photo, _planet);
        var pp = new PPDocument(_name, _photo, _planet, _timeLines.Month);
        var pms = new PMSDocument(_name, _photo);
        
        switch (_timeLine)
        {
            case TimeLine.Void:
                _origin = Convert.ToBoolean(_rnd.Next(0, 1));
                if (!_origin)
                {
                    iic.Randomize(1);
                    pp.Randomize(1);
                    pms.Randomize(1);
                }
                break;
            case TimeLine.Eternity:
                _origin = false;
                iic.Randomize(1);
                pp.Randomize(1);
                pms.Randomize(1);
                break;
            case TimeLine.Scary:
                _origin = true;
                break;
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
