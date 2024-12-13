using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Text")]
    public TimeLine NPCTimeLine;
    [Header("Docs")]
    [SerializeField] private GameObject _PMS;
    [SerializeField] private GameObject _IIC;
    [SerializeField] private GameObject _PP;
    
    [Header("Valuable")]
    [SerializeField] private uint _cost;

    public bool FaceChanged { get; set; }
    public bool IsCriminal  { get; private set; }
    public Sprite Photo { get; set; }
    
    private string _name;
    private int _planet;
    private List<GameObject> _collectedDocs = new();
    private int _docsCount = 1;
    
    public List<DialogFragment> Fragments { get; set; }= new();
    private DialogFragment _endPassText, _endBackText;
    
    private bool _origin, _docsGiven;
    private CheckState _checkState = CheckState.None;
    
    private TimeLines _timeLines => FindFirstObjectByType<TimeLines>();
    private DialogSystem _dialogSys => FindFirstObjectByType<DialogSystem>();
    private NPCManager _npcManager => FindFirstObjectByType<NPCManager>();
    
    private void Start()
    {
        if (NPCTimeLine == TimeLine.Void)
        {
            var randomDialog = Random.Range(0,RandomParamSt.NormalConfigs.Count);
            Fragments = RandomParamSt.NormalConfigs[randomDialog].Fragments;
            _endPassText = RandomParamSt.NormalConfigs[randomDialog].GoFragment;
            _endBackText = RandomParamSt.NormalConfigs[randomDialog].BackFragment;
        }


        if (Random.Range(0,101) < _npcManager.CriminalChance)
        {
            _name = _npcManager.ChangedCriminals[Random.Range(0,_npcManager.ChangedCriminals.Count)];
            IsCriminal = true;
            _cost *= 2;
            _origin = false;
            _npcManager.ChangedCriminals.Remove(_name);
        }
        else
        {
            var newList = (RandomParamSt.Names.Except(_npcManager.ChangedCriminals)).ToList();
            _name = newList[Random.Range(0, newList.Count)];
        }
        
        _planet = Random.Range(1,5);

        if (_timeLines.WeekDate > 3)
            _docsCount = 3;
        else if (_timeLines.WeekDate > 1)
            _docsCount = 2;
    }

    public void GiveDocs()
    {
        if (_docsGiven) return;
        _docsGiven = true;
        
        Document pp = null, iic = null;
        var pms = _npcManager.GiveDoc(_PMS, 1);
        pms.Initialize(_name, Photo, _planet);
        if (_docsCount > 1)
        {
            iic = _npcManager.GiveDoc(_IIC, 2);
            iic.Initialize(_name, Photo, _planet);
        }

        if (_docsCount > 2)
        {
            pp = _npcManager.GiveDoc(_PP, 3);
            pp.Initialize(_name, Photo, _planet);
        }
        
        if(!IsCriminal) switch (NPCTimeLine)
        {
            case TimeLine.Void:
                _origin = Random.Range(0,2) == 1;
                if (!_origin)
                {
                    iic?.Randomize(2);
                    pp?.Randomize(2);
                    pms.Randomize(2);
                }
                break;
            case TimeLine.Eternity:
                _origin = false;
                iic?.Randomize(2);
                pp?.Randomize(2);
                pms.Randomize(2);
                break;
            case TimeLine.Robots:
                _origin = true;
                break;
        }
        print("IsCriminal " + IsCriminal);
        print("FaceChanged " + FaceChanged);
        print("Origin " + _origin);
    }

    public void Check(bool playerOrigin)
    {
        if (_origin == playerOrigin)
        {
            _timeLines.ChangeTimeline(TimeLine.Void);
            TimeLines.CorrectNPC += _cost;
        }
        else
        {
            TimeLines.WrongNPC += _cost;
            _timeLines.ChangeTimeline(TimeLine.Void, false);
            if(NPCTimeLine != TimeLine.Void)
                _timeLines.ChangeTimeline(NPCTimeLine);
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
        else if(_checkState == CheckState.None)
        {
            _dialogSys.FragmentsStack = Fragments.ToList();
        }
        _dialogSys.PlayNext();
        
    }
    
}

