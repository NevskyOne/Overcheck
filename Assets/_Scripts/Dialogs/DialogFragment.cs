using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogFragment
{
    [TextArea] public string Text;
    public GameObject Object;
    public List<ButtonSt> Buttons;
}
