using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [HideInInspector] public List<DialogFragment> FragmentsStack = new();

    [Header("Animation")]
    [SerializeField] private float _letterDelay = 0.1f;
    [SerializeField] private float _startFontSize = 10f;
    [SerializeField] private float _endFontSize = 30f;
    [SerializeField] private float _growDuration = 0.5f; 
    [Header("Text")]
    [SerializeField] private GameObject _dialogMenu;
    [SerializeField] private TMP_Text _textField;

    [Header("Buttons")] 
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buttonsHolder;

    [Header("Object")] [SerializeField] private Transform _objHolder;
    
    private PlayerMovement _playerMovement => FindFirstObjectByType<PlayerMovement>();
    private PlayerInteractions _playerInter => FindFirstObjectByType<PlayerInteractions>();
    private NPCManager _npcManager => FindFirstObjectByType<NPCManager>();
    
    private string _currentLine = "";
    
    public event Action ChatEnded;
    public CheckState GoAfter;

    public void PlayNext()
    {
        if (_currentLine != "" && _textField.text != _currentLine)
        {
            StopAllCoroutines();
            _textField.text = "";
            _textField.text = _currentLine;
            _npcManager.SetNPCTalking(false);
        }
        else if (FragmentsStack.Count > 0)
        {
            _npcManager.SetNPCTalking();
            _dialogMenu.SetActive(true);
            _playerInter.StopFocus();
            for (var i = 0; i < _buttonsHolder.childCount; i++ )    
            {
                Destroy(_buttonsHolder.GetChild(0).gameObject);
            }
            PlayFragment(FragmentsStack[0]);
            _currentLine = FragmentsStack[0].Text;
            FragmentsStack.RemoveAt(0);
            
        }
        else
        {
            EndChat();
        }
    }
    public void PlayFragment(DialogFragment fragment)
    {
        StartCoroutine(AnimateText(fragment.Text));

        ShowButtons(new (fragment.Buttons));
        PlaceObj(fragment.Object);
        if (fragment.GoAfter)
        {
            GoAfter = CheckState.Correct;
        }
        if (fragment.GiveDocs)
        {
            NPCManager.CurrentNPC.GiveDocs();
        }
        
    }

    private void ShowButtons(List<ButtonSt> buttons)
    {
        foreach (var btn in buttons)
        {
            var _newButton =Instantiate(_buttonPrefab, _buttonsHolder);
            var component = _newButton.GetComponent<DialogButton>();
            component.ButtonFields = btn;
        }
    }
    
    private void PlaceObj(GameObject obj)
    {
        if(obj) Instantiate(obj, _objHolder);
    }

    public void EndChat()
    {
        StopAllCoroutines();
        _currentLine = "";
        _npcManager.SetNPCTalking(false);
        
        _playerMovement.enabled = true;
        FragmentsStack.Clear();
        _playerInter.Focus();
        
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

    private IEnumerator AnimateText(string textToDisplay)
    {
        // Пустой массив для построения текста
        char[] displayedText = new char[textToDisplay.Length];
        for (int i = 0; i < displayedText.Length; i++) displayedText[i] = ' ';

        for (int i = 0; i < textToDisplay.Length; i++)
        {
            // Добавляем символ в массив и запускаем анимацию увеличения
            displayedText[i] = textToDisplay[i];
            StartCoroutine(AnimateCharacterSize(displayedText, i));

            // Задержка перед добавлением следующего символа
            yield return new WaitForSeconds(_letterDelay);
        }
    }

    private IEnumerator AnimateCharacterSize(char[] displayedText, int charIndex)
    {
        float elapsedTime = 0f;

        while (elapsedTime < _growDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _growDuration);

            // Формируем строку с увеличением только текущего символа
            string modifiedText = "";
            for (int i = 0; i < displayedText.Length; i++)
            {
                if (i == charIndex && displayedText[i] != ' ')
                {
                    // Увеличиваем текущий символ
                    modifiedText += $"<size={(int)Mathf.Lerp(_startFontSize, _endFontSize, t)}>{displayedText[i]}</size>";
                }
                else if (displayedText[i] != ' ')
                {
                    // Остальные символы остаются неизменными
                    modifiedText += $"<size={_endFontSize}>{displayedText[i]}</size>";
                }
                else
                {
                    // Добавляем пустое место для символов, которые ещё не появились
                    modifiedText += " ";
                }
            }

            // Обновляем текст
            _textField.text = modifiedText;
            yield return null;
        }
        

        _textField.text = _currentLine;
    }
}
