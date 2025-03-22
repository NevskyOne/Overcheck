using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BrokenElectricityCore : MonoBehaviour,IQuiz
{
    [SerializeField] private List<FormulaSO> _formulas = new();
    [SerializeField] private List<BrokenElectricityStartBtn> _startBtns = new();
    [SerializeField] private List<BrokenElectricityEndBtn> _endBtns = new();
    [SerializeField] private List<Material> _materials;
    [SerializeField] private LineRenderer[] _lines;

    [Inject] private EventBus _eventBus;
    
    public void StartQuiz(IQuizData data)
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
            Reset();
        else
            Win();
    }

    private void Reset()
    {
        foreach (var b in _startBtns)
        {
            b.Reset();
        }
    }

    public void Win()
    {
        _eventBus.Invoke(new WinEvent());
    }
    public void Lose()
    {
        _eventBus.Invoke(new LoseEvent());
    }
}