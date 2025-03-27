using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogFragment
{
    [TextArea] public string Text;
    [SerializeReference,SerializeReferenceButton] public IDialogAction[] Actions;
    public ButtonSt[] Buttons;
}
