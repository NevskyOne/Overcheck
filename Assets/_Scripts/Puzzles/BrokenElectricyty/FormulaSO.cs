using UnityEngine;

[CreateAssetMenu(fileName = "NewFormula", menuName = "Formula")]
public class FormulaSO : ScriptableObject
{
    [SerializeField] private string _firstPart;
    [SerializeField] private string _secondPart;

    public string FirstPart => _firstPart;
    public string SecondPart => _secondPart;
}