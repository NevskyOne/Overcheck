using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogButton : MonoBehaviour
{
    public ButtonSt ButtonFields;
    private DialogSystem _dialogSys;

    private void Start()
    {
        GetComponentInChildren<TMP_Text>().text = ButtonFields.Text;
        _dialogSys = FindFirstObjectByType<DialogSystem>();
        
    }

    public void Click()
    {
        _dialogSys.FragmentsStack = ButtonFields.Fragments.ToList();
        _dialogSys.PlayNext();
    }
}

[Serializable]
public struct ButtonSt
{
    public string Text;
    public List<DialogFragment> Fragments;
}
