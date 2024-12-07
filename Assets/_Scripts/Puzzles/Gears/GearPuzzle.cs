using System.Collections.Generic;
using UnityEngine;

public class GearPuzzle : Puzzle
{
    [SerializeField] private List<GearPuzzlePreset> _presets = new();
    
    private GearPuzzlePreset _currentPreset;
    
    public override void StartPuzzle()
    {
        var preset = _presets[Random.Range(0, _presets.Count)];
        _currentPreset = Instantiate(preset.gameObject, transform).GetComponent<GearPuzzlePreset>();
        _currentPreset.Initialize(GetComponent<Canvas>());
    }

    public void CheckPuzzle()
    {
        SolvePuzzle();
    }
}