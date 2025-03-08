using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DialogSystem : MonoBehaviour
{
    [HideInInspector] public List<DialogFragment> FragmentsStack = new();

    [Header("Animation")]
    [SerializeField] private float _letterDelay = 0.1f;
    [SerializeField] private float _startFontSize = 10f;
    [SerializeField] private float _endFontSize = 30f;
    [SerializeField] private float _growDuration = 0.5f; 
    [Header("Text")]
    public GameObject DialogMenu;
    public TMP_Text TextField;
    [Header("Buttons")] 
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buttonsHolder;
    [Header("Audio")] [SerializeField] private AudioSource _source;
    
    private Image _frameImg => DialogMenu.GetComponent<Image>();
    private PlayerMovement _playerMovement => Player.Movement;
    private PlayerInteractions _playerInter => Player.Interactions;
    private DocumentControlService _docControl;
    
    private string _currentLine = "";
    private NPCAnim _npcAnim;
    private List<IDialogAction> _actions = new List<IDialogAction>();
    
    public event Action ChatEnded;
    public static CheckState GoAfter;

    [Inject]
    private void Initialize(DocumentControlService docControl)
    {
        _docControl = docControl;
    }

    public void PlayNext(NPCAnim npcAnim = null)
    {
        if(npcAnim)
            _npcAnim = npcAnim;
        if (_currentLine != "" && TextField.text != _currentLine)
        {
            StopAllCoroutines();
            TextField.text = "";
            TextField.text = _currentLine;
            _npcAnim.IsTalking = true;
            _source.mute = true;
        }
        else if (FragmentsStack.Count > 0)
        {
            TextField.color = Color.white;
            _frameImg.color = Color.white;
            TextField.alignment = TextAlignmentOptions.TopLeft;
            _npcAnim.IsTalking = false;
            DialogMenu.SetActive(true);
            _playerInter.StopFocus();
            
            PlayFragment(FragmentsStack[0]);
            _currentLine = FragmentsStack[0].Text;
            FragmentsStack.RemoveAt(0);
            
        }
        else
        {
            foreach (var action in _actions)
            {
                action?.AfterAction();
            }

            _actions = new List<IDialogAction>();
            EndChat();
        }
    }

    private void PlayFragment(DialogFragment fragment)
    {
        StartCoroutine(AnimateText(fragment.Text));
        _source.mute = false;
        foreach (Transform child in _buttonsHolder)
        {
            Destroy(child.gameObject);
        }
        if(fragment.Buttons.Count > 0)
            ShowButtons(new (fragment.Buttons));

        if (fragment.Actions == null) return;
        foreach (var action in fragment.Actions)
        {
            action?.DoAction();
            _actions.Add(action);
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

    public void EndChat()
    {
        StopAllCoroutines();
        _actions = new List<IDialogAction>();
        _currentLine = "";
        
        _npcAnim.IsTalking = false;
        _npcAnim = null;
        
        _source.mute = true;

        Player.State = PlayerState.Movement;
        
        _playerMovement.Enable();
        FragmentsStack.Clear();
        _playerInter.Focus();
        
        for (var i = 0; i < _buttonsHolder.childCount; i++ )    
        {
            Destroy(_buttonsHolder.GetChild(i).gameObject);
        }
        DialogMenu.SetActive(false);
        
        switch (GoAfter)
        {
            case CheckState.Correct:
                _docControl.Accept();
                break;
            case CheckState.Wrong:
                _docControl.Reject();
                break;
        }
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
        _npcAnim.IsTalking = false;
        _source.mute = true;
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
            TextField.text = modifiedText;
            yield return null;
        }

        TextField.text = _currentLine;
    }
}
