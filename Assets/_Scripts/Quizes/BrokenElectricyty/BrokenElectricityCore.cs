using System.Collections.Generic;
using UnityEngine;

public class BrokenElectricityCore : IQuiz
{
    [SerializeField] private List<FormulaSO> _formulas = new();
    [SerializeField] private List<BrokenElectricityStartBtn> _startBtns = new();
    [SerializeField] private List<BrokenElectricityEndBtn> _endBtns = new();
    [SerializeField] private List<Material> _materials;
    [SerializeField] private LineRenderer[] _lines;
    
    public void StartQuiz(QuizData data)
    {
        foreach (var line in _lines)
        {
            var mat = _materials[Random.Range(0,_materials.Count)];
            line.material = mat;
            _materials.Remove(mat);
        }
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
            Lose();
        else
            Win();
    }

    public void Win(){}
    public void Lose()
    {
        foreach (var b in _startBtns)
        {
            b.Reset();
        }
    }
}