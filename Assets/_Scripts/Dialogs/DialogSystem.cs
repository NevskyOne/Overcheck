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
    
    private PlayerMovement _playerMovement => FindFirstObjectByType<PlayerMovement>();
    public event Action ChatEnded;
    public CheckState GoAfter;
    private NPCManager _npcManager;

    private void Start()
    {
        _npcManager = FindFirstObjectByType<NPCManager>();
    }

    public void PlayNext()
    {
        if (FragmentsStack.Count > 0)
        {
            _playerMovement.enabled = false;
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
    public void PlayFragment(DialogFragment fragment)
    {
        _textField.text = fragment.Text;
        ShowButtons(fragment.Buttons);
        PlaceObj(fragment.Object);
        if(fragment.GiveDocs)
            _npcManager.CurrentNPC.GiveDocs();
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
        _playerMovement.enabled = true;
        FragmentsStack.Clear();
        for (var i = 0; i < _buttonsHolder.childCount; i++ )    
        {
            Destroy(_buttonsHolder.GetChild(i).gameObject);
        }
        _dialogMenu.SetActive(false);
        
        if(GoAfter == CheckState.Correct)
            _npcManager.GoTowards();
        else if(GoAfter == CheckState.Wrong)
            _npcManager.GoBack();
        GoAfter = CheckState.None;
        
        ChatEnded?.Invoke();
    }
}
