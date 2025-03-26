using TMPro;
using UnityEngine;
using Zenject;

public class PMSDocument : Document, IAcceptable
{
    [SerializeField] private TMP_Text _genderText;
    [Header("Particles")]
    [SerializeField] private GameObject _acceptParticle;
    [SerializeField] private GameObject _rejectParticle;
    [SerializeField] private Transform _particleSpawn;

    private DocumentControlService _docControl;
    
    [Inject]
    private void Initialize(DocumentControlService docControl)
    {
        _docControl = docControl;
    }
    
    public override void Setup(DocData docData)
    {
        base.Setup(docData);
        
        PMSData pmsData = docData as PMSData;

        _genderText.text = pmsData.Male ? "Мужчина" : "Женщина";
    }

    public void Accept()
    {
        if (_docControl.CurrentNPC.State != CheckState.None) return;
        Instantiate(_acceptParticle, _particleSpawn);
        _docControl.CurrentNPC.State = CheckState.Correct;
    }
    
    public void Reject()
    {
        if (_docControl.CurrentNPC.State != CheckState.None) return;
        Instantiate(_rejectParticle, _particleSpawn);
        _docControl.CurrentNPC.State = CheckState.Wrong;
    }
}