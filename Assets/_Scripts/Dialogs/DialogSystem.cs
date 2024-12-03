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
    [SerializeField] private float _sizeIncreaseDuration = 0.3f;
    [SerializeField] private float _sizeMultiplier = 1.5f;
    [Header("Text")]
    [SerializeField] private GameObject _dialogMenu;
    [SerializeField] private TMP_Text _textField;

    [Header("Buttons")] 
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buttonsHolder;

    [Header("Object")] [SerializeField] private Transform _objHolder;
    
    private PlayerMovement _playerMovement => FindFirstObjectByType<PlayerMovement>();
    private PlayerInteractions _playerInter => FindFirstObjectByType<PlayerInteractions>();
    private CameraManager _cameraMng =>  FindFirstObjectByType<CameraManager>();
    private NPCManager _npcManager => FindFirstObjectByType<NPCManager>();

    private Coroutine _printRoutine;
    
    public event Action ChatEnded;
    public CheckState GoAfter;

    public void PlayNext()
    {
        if (FragmentsStack.Count > 0)
        {
            _dialogMenu.SetActive(true);
            _playerInter.StopFocus();
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
        if(_printRoutine != null)
            StopCoroutine(_printRoutine);
        _printRoutine = StartCoroutine(PrintRoutine(fragment.Text));
        
        ShowButtons(fragment.Buttons);
        PlaceObj(fragment.Object);
        if(fragment.GiveDocs)
            NPCManager.CurrentNPC.GiveDocs();
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
        if(obj) Instantiate(obj, _objHolder);
    }

    public void EndChat()
    {
        _playerMovement.enabled = true;
        FragmentsStack.Clear();
        _playerInter.Focus();
        if(_cameraMng.transform.localPosition != new Vector3(0,0.64f,0))
            _cameraMng.ResetCamera();
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

    private IEnumerator PrintRoutine(string text)
    {
        _npcManager.SetNPCTalking();
        _textField.text = text; // Устанавливаем текст
        _textField.ForceMeshUpdate(); // Обновляем данные для вершин

        TMP_TextInfo textInfo = _textField.textInfo;

        // Скрываем весь текст через альфу
        var colors = textInfo.meshInfo[0].colors32; // Цвета вершин текста
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (textInfo.characterInfo[i].isVisible)
            {
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                for (int j = 0; j < 4; j++)
                {
                    colors[vertexIndex + j].a = 0; // Прозрачность
                }
            }
        }
        _textField.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // Последовательно отображаем символы
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue; // Пропускаем невидимые символы (например, пробелы)

            yield return StartCoroutine(AnimateSingleLetter( i, _sizeIncreaseDuration, _sizeMultiplier));

            yield return new WaitForSeconds(_letterDelay);
        }
        _npcManager.SetNPCTalking(false);
    }
    
    private IEnumerator AnimateSingleLetter(int charIndex, float duration, float sizeMultiplier)
    {
        TMP_TextInfo textInfo = _textField.textInfo;

        // Получаем информацию о вершинах текущего символа
        Vector3[] vertices = textInfo.meshInfo[textInfo.characterInfo[charIndex].materialReferenceIndex].vertices;
        Color32[] colors = textInfo.meshInfo[textInfo.characterInfo[charIndex].materialReferenceIndex].colors32;

        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        Vector3 charMidBasline = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

        float elapsedTime = 0f;

        // Плавное увеличение символа
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(1f, sizeMultiplier, elapsedTime / duration);

            // Применяем масштабирование только к текущему символу
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = charMidBasline + (vertices[vertexIndex + j] - charMidBasline) * scale;
                colors[vertexIndex + j].a = (byte)Mathf.Lerp(0, 255, elapsedTime / duration); // Прозрачность
            }

            _textField.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            yield return null;
        }

        // Финальный вид
        for (int j = 0; j < 4; j++)
        {
            vertices[vertexIndex + j] = charMidBasline + (vertices[vertexIndex + j] - charMidBasline) * sizeMultiplier;
            colors[vertexIndex + j].a = 255; // Полная непрозрачность
        }

        _textField.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

    }
}
