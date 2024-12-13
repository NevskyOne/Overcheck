using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogFragment
{
    [TextArea] public string Text;
    [SerializeReference,SerializeReferenceButton] public List<IDialogAction> Actions;
    public List<ButtonSt> Buttons;
}
