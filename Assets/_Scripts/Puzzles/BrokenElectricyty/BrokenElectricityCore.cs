using System.Collections.Generic;
using UnityEngine;

public class BrokenElectricityCore : MonoBehaviour
{
    [SerializeField] private GameObject _puzzleSolvedIndicator;
    [SerializeField] private List<FormulaSO> _formulas = new();
    [SerializeField] private List<BrokenElectricityStartBtn> _startBtns = new();
    [SerializeField] private List<BrokenElectricityEndBtn> _endBtns = new();
    
    private RandomEvents _randomEvents;
    private bool _isPuzzleSolved = false;

    private void Start()
    {
        _randomEvents = FindFirstObjectByType<RandomEvents>();
    }
    
    public void StartPuzzle()
    {
        var endBtns = _endBtns;
        var formulas = _formulas;
        
        foreach (var b in _startBtns)
        {
            var formula = formulas[Random.Range(0, formulas.Count)];
            var endBtn = endBtns[Random.Range(0, endBtns.Count)];
            
            b.SetProperties(endBtn, formula);
            b.OnEndDragEvent += OnBtnDragEnd;
            
            formulas.Remove(formula);
            endBtns.Remove(endBtn);
        }
    }

    private void OnBtnDragEnd()
    {
        var allBtnsConnected = true;

        foreach (var b in _startBtns)
        {
            if (!b.IsConnected)
                allBtnsConnected = false;
        }
        
        if (!allBtnsConnected) return;

        var isSolved = true;
        
        foreach (var b in _startBtns)
        {
            if (!b.IsSolved)
                isSolved = false;
        }

        if (!isSolved)
        {
            foreach (var b in _startBtns)
            {
                b.Reset();
            }
        }
        else
        {
            _isPuzzleSolved = true;
            _randomEvents.OnEventEnd();
        }
    }
}