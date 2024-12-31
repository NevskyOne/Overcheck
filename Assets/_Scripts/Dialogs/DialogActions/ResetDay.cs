using System;
using UnityEngine;

[Serializable]
public class ResetDay : IDialogAction
{
    public void DoAction() { }

    public void AfterAction()
    {
        GameObject.FindFirstObjectByType<Robot>().Deactivate();
    }
}
