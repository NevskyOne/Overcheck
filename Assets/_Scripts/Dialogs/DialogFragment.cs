using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogFragment
{
    [TextArea] public string Text;
    public bool GiveDocs;
    public GameObject Object;
    public List<ButtonSt> Buttons;
}
