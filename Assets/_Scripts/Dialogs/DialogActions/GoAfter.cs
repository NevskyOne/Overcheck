using System;
using UnityEngine;

[Serializable]
public class CoAfter : IDialogAction
{
    private DialogSystem _dialogSystem => GameObject.FindFirstObjectByType<DialogSystem>();
    
    public void DoAction()
    {
        _dialogSystem.GoAfter = CheckState.Correct;
    }
    public void AfterAction(){}
}