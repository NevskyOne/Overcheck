using System;
using UnityEngine;

[Serializable]
public class GiveDocs : IDialogAction
{
    public void DoAction() => GameObject.FindFirstObjectByType<DocumentControlService>().GiveDocs();
    public void AfterAction(){}
}