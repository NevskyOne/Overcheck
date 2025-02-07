using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ColorFrame : IDialogAction
{
    [SerializeField] private Color _color;
    private DialogSystem _dialogSystem => GameObject.FindFirstObjectByType<DialogSystem>();
    
    public void DoAction()
    {
        _dialogSystem.TextField.color = _color;
        _dialogSystem.TextField.alignment = TextAlignmentOptions.Center;
        _dialogSystem.DialogMenu.GetComponent<Image>().color = _color;
    }
    public void AfterAction(){}
}
