using TMPro;
using UnityEngine;

public class BrokenElectricityEndBtn : MonoBehaviour
{
    [SerializeField] private TMP_Text _formulaText;
    public GameObject VFX;
    public Transform LinePos;
    public string Formula { get; private set; }
    
    public void SetFormula(string formula)
    {
        _formulaText.text = formula;
        Formula = formula;
    }
}