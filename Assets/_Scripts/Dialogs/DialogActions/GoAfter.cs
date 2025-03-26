using System;

[Serializable]
public class CoAfter : IDialogAction
{
    public void DoAction()
    {
        DialogSystem.GoAfter = CheckState.Correct;
    }
    public void AfterAction(){}
}