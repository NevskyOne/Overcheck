using System.Collections.Generic;
using UnityEngine;

public class GearPuzzle : MonoBehaviour, IQuiz
{
    [SerializeField] private List<GearPuzzlePreset> _presets = new();
    
    private GearPuzzlePreset _currentPreset;
    
    public void StartQuiz(QuizData data)
    {
        var preset = _presets[Random.Range(0, _presets.Count)];
        _currentPreset = Instantiate(preset.gameObject, transform).GetComponent<GearPuzzlePreset>();
        _currentPreset.Initialize(GetComponent<Canvas>());
    }

    public void CheckPuzzle()
    {
        _currentPreset.ResetGears();
        Destroy(_currentPreset.gameObject);
        Win();
    }
    
    public void Win(){}
    public void Lose(){}
}