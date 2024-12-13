using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogConfig", menuName = "Scriptable/DialogConfig")]
public class DialogConfig : ScriptableObject
{
    public List<DialogFragment> Fragments;
    public DialogFragment GoFragment;
    public DialogFragment BackFragment;
}