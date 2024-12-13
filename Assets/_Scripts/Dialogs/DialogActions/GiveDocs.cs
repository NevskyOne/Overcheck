using System;

[Serializable]
public class GiveDocs : IDialogAction
{
    public void DoAction() => NPCManager.CurrentNPC.GiveDocs();
    
}