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
    
    public Goal CurrentGoal { get; set; }
    
    private string _name;
    private int _planet;
    private List<GameObject> _collectedDocs = new();
    private int _docsCount = 1;
    
    private Random _rnd = new Random();
    private bool _origin;
    private CheckState _checkState = CheckState.None;
    
    private TimeLines _timeLines;
    private DialogSystem _dialogSys;
    private NPCManager _npcManager;
    
    private void Start()
    {
        _timeLines = FindFirstObjectByType<TimeLines>();
        _dialogSys = FindFirstObjectByType<DialogSystem>();
        _npcManager = FindFirstObjectByType<NPCManager>();
        transform.GetChild(0).GetComponent<MeshFilter>().mesh = _models[_rnd.Next(0, 3)];
        
        _name = RandomParamSt.Names[_rnd.Next(0,RandomParamSt.Names.Length)];
        _photo = RandomParamSt.Photos[_rnd.Next(0,RandomParamSt.Photos.Length)];
        _planet = _rnd.Next(1,5);

        if (_timeLines.WeekDate > 4)
            _docsCount = 3;
        else if (_timeLines.WeekDate > 2)
            _docsCount = 2;
    }

    public void GiveDocs()
    {
        Document pp = null, iic = null;
        var pms = _npcManager.GiveDoc(_PMS, 1);
        pms.Initialize(_name, _photo, _planet);
        if (_docsCount > 1)
        {
            iic = _npcManager.GiveDoc(_IIC, 2);
            iic.Initialize(_name, _photo, _planet);
        }

        if (_docsCount > 2)
        {
            pp = _npcManager.GiveDoc(_PP, 3);
            pp.Initialize(_name, _photo, _planet);
        }

        switch (_timeLine)
        {
            case TimeLine.Void:
                _origin = Convert.ToBoolean(_rnd.Next(0, 2));
                if (!_origin)
                {
                    if (_docsCount > 1)
                        iic.Randomize(1);
                    if (_docsCount > 2)
                        pp.Randomize(1);
                    pms.Randomize(1);
                }
                break;
            case TimeLine.Eternity:
                _origin = false;
                if (_docsCount > 1)
                    iic.Randomize(1);
                if (_docsCount > 2)
                    pp.Randomize(1);
                pms.Randomize(1);
                break;
            case TimeLine.Father:
                _origin = true;
                break;
        }
        print(_origin);
    }

    public void Check(bool playerOrigin)
    {
        if ((_origin && playerOrigin && CurrentGoal == _targetGoal) || (_origin == false && playerOrigin == false))
        {
            _timeLines.CorrectNPC += _cost;
            _timeLines.ChangeTimeline(TimeLine.Void);
        }
        else if (_origin && playerOrigin)
        {
            _timeLines.ChangeTimeline(TimeLine.Void);
            _timeLines.CorrectNPC += (uint)(_cost * 0.75f);
        }
        else
        {
            _timeLines.WrongNPC += _cost;
            _timeLines.ChangeTimeline(TimeLine.Void, false);
            if(_timeLine != TimeLine.Void)
                _timeLines.ChangeTimeline(_timeLine);
        }

        _checkState = playerOrigin ? CheckState.Correct : CheckState.Wrong;
    }

    public void CollectDoc(GameObject doc, bool add = true)
    {
        if(add)
            _collectedDocs.Add(doc);
        else
            _collectedDocs.Remove(doc);
    }
    
    public void StartChat()
    {
        if (_collectedDocs.Count == _docsCount && _checkState == CheckState.Correct)
        {
            _dialogSys.FragmentsStack = new List<DialogFragment>{_endPassText};
            foreach (var doc in _collectedDocs)
                Destroy(doc);
            _collectedDocs.Clear();
            _dialogSys.GoAfter = _checkState;
        }
        else if (_collectedDocs.Count == _docsCount && _checkState == CheckState.Wrong)
        {
            _dialogSys.FragmentsStack = new List<DialogFragment>{_endBackText};
            foreach (var doc in _collectedDocs)
                Destroy(doc);
            _collectedDocs.Clear();
            _dialogSys.GoAfter = _checkState;
        }
        else
        {
            _dialogSys.FragmentsStack = _fragments.ToList();
        }
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
