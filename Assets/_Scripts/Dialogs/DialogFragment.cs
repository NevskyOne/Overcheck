using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogFragment
{
    [TextArea] public string Text;
    public bool GiveDocs;
    public bool GoAfter;
    public GameObject Object;
    public List<ButtonSt> Buttons;
}
