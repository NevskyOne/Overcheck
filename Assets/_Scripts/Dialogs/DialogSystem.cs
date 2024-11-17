using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [HideInInspector] public List<DialogFragment> FragmentsStack = new();
    
    [Header("Text")]
    [SerializeField] private GameObject _dialogMenu;
    [SerializeField] private TMP_Text _textField;

    [Header("Buttons")] 
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buttonsHolder;

    [Header("Object")] [SerializeField] private Transform _objHolder;
    
    public event Action ChatEnded;
    public void PlayNext()
    {
        if (FragmentsStack.Count > 0)
        {
            _dialogMenu.SetActive(true);
            for (var i = 0; i < _buttonsHolder.childCount; i++ )    
            {
                Destroy(_buttonsHolder.GetChild(i).gameObject);
            }
            PlayFragment(FragmentsStack[0]);
            FragmentsStack.RemoveAt(0);
        }
        else
        {
            EndChat();
        }
    }
    private void PlayFragment(DialogFragment fragment)
    {
        _textField.text = fragment.Text;
        ShowButtons(fragment.Buttons);
        PlaceObj(fragment.Object);
    }

    private void ShowButtons(List<ButtonSt> buttons)
    {
        foreach (var btn in buttons)
        {
            var _newButton =Instantiate(_buttonPrefab, _buttonsHolder);
            var component = _newButton.AddComponent<DialogButton>();
            component.ButtonFields = btn;
        }
    }
    
    private void PlaceObj(GameObject obj)
    {
        if(obj != null) Instantiate(obj, _objHolder);
    }

    public void EndChat()
    {
        FragmentsStack.Clear();
        for (var i = 0; i < _buttonsHolder.childCount; i++ )    
        {
            Destroy(_buttonsHolder.GetChild(i).gameObject);
        }
        _dialogMenu.SetActive(false);
        ChatEnded?.Invoke();
    }
}
