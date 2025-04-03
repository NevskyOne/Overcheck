using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TutorialService : MonoBehaviour
{
    [SerializeField] private GameObject _movement;
    [SerializeField] private GameObject _usable;
    [SerializeField] private GameObject _holding;
    [SerializeField] private List<GameObject> _markers = new List<GameObject>();
    [SerializeField] private GameObject _intro;

    private MainUI _mainUI;
    
    [Inject]
    private async void Initialize(MainUI ui)
    {
        _mainUI = ui;
        if (PlayerPrefs.GetInt("IsTutored") == 1)
        {
            //_intro.SetActive(true);
            await Task.Delay(23000);
            //_intro.SetActive(false);
            StartCoroutine(TutorRoutine());
            PlayerPrefs.SetInt("IsTutored", 1);
        }
    }

    private IEnumerator TutorRoutine()
    {
        foreach (var marker in _markers)
        {
            marker.SetActive(true);
            switch (_markers.IndexOf(marker))
            {
                case 1:
                    _movement.SetActive(true);
                    break;
                case 2:
                    _mainUI.ShowPopup("Отлично!");
                    _movement.SetActive(false);
                    _usable.SetActive(true);
                    break;
                case 3:
                    _mainUI.ShowPopup("Замечательно!");
                    _usable.SetActive(false);
                    break;
                case 4:
                    _mainUI.ShowPopup("Вау!");
                    break;
                case 5:
                    _mainUI.ShowPopup("Круто.");
                    break;
                case 6:
                    _mainUI.ShowPopup("Просто умничка!");
                    break;
            }
            yield return new WaitUntil(() => !marker.activeSelf);
        }
    }
}
