using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class NPC : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Mesh[] _models;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Material[] _acesMaterials;
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
    
    private Sprite _photo;
    private string _name;
    private int _planet;
    private List<GameObject> _collectedDocs = new();
    private int _docsCount = 1;
    
    private List<DialogFragment> _fragments = new();
    private DialogFragment _endPassText, _endBackText;
    
    private Random _rnd = new Random();
    private bool _origin, _docsGiven;
    private CheckState _checkState = CheckState.None;
    
    private TimeLines _timeLines => FindFirstObjectByType<TimeLines>();
    private DialogSystem _dialogSys => FindFirstObjectByType<DialogSystem>();
    private NPCManager _npcManager => FindFirstObjectByType<NPCManager>();
    
    private void Start()
    {
        var randomDialog = _rnd.Next(DialogParser.ParsedFragments.Count);
        _fragments = DialogParser.ParsedFragments[randomDialog];
        _endPassText = DialogParser.PassFragmenst[randomDialog];
        _endBackText = DialogParser.NotPassFragmenst[randomDialog];
        

        var meshRenderer = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();
        var randomModel = _rnd.Next(0, 10);
        meshRenderer.sharedMesh = _models[randomModel];

        var randomMat = _rnd.Next(0, _materials.Length);
        meshRenderer.SetMaterials(new()
        {
            _materials[randomMat],
            _materials[randomMat],
            _acesMaterials[_rnd.Next(0, _acesMaterials.Length)]
        });

        _photo = RandomParamSt.Photos[(randomModel+1) * 10 + (int)Mathf.Ceil((randomMat+1)/3f)];
        
        int randomHat = _rnd.Next(0, 4);
        if (randomHat < 3)
            meshRenderer.SetBlendShapeWeight(randomHat, 100);
        int randomCount = _rnd.Next(0, 5);
        if (randomCount > 0)
        {
            for (var i = 0; i < randomCount; i++)
                meshRenderer.SetBlendShapeWeight(_rnd.Next(3, 7), 100);
        }
    

        if (_rnd.Next(101) < _npcManager.CriminalChance)
        {
            _name = _npcManager.ChangedCriminals[_rnd.Next(0,_npcManager.ChangedCriminals.Count)];
            IsCriminal = true;
            _cost *= 2;
            _origin = false;
            _npcManager.ChangedCriminals.Remove(_name);
        }
        else
        {
            var newList = (RandomParamSt.Names.Except(_npcManager.ChangedCriminals)).ToList();
            _name = newList[_rnd.Next(0, newList.Count)];
        }
        
        _planet = _rnd.Next(1,8);

        if (_timeLines.WeekDate > 3)
            _docsCount = 3;
        else if (_timeLines.WeekDate > -1)
            _docsCount = 2;
    }

    public void GiveDocs()
    {
        if (_docsGiven) return;
        _docsGiven = true;
        
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
        
        if(!IsCriminal) switch (NPCTimeLine)
        {
            case TimeLine.Void:
                _origin = _rnd.Next(2) == 1;
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
            _timeLines.CorrectNPC += _cost;
        }
        else
        {
            _timeLines.WrongNPC += _cost;
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
        else
        {
            _dialogSys.FragmentsStack = _fragments.ToList();
        }
        _dialogSys.PlayNext();
        
    }
    
}

