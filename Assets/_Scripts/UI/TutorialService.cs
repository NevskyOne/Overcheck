
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class TutorialService : MonoBehaviour
{
    [SerializeField] private GameObject _movement;
    [SerializeField] private GameObject _usable;
    [SerializeField] private GameObject _holding;
    [SerializeField] private List<GameObject> _markers = new ();
    [SerializeField] private GameObject _intro;
    [SerializeField] private bool _startOnFirst;
    private MainUI _mainUI;
    private string[] _phrases = {"Отлично!", "Круто.","Умничка!","Вау!","Замечательно!","Великолепно!","Бесподобно."};
    
    [Inject]
    private async void Initialize(MainUI ui)
    {
        _mainUI = ui;
        
        if ((PlayerPrefs.GetInt("IsTutored") == 0 && _startOnFirst) || !_startOnFirst )
        {
            _intro.SetActive(true);
            await Task.Delay(23000);
            _intro.SetActive(false);
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
                    _mainUI.ShowPopup(_phrases[Random.Range(0,_phrases.Length)]);
                    _movement.SetActive(false);
                    _usable.SetActive(true);
                    break;
                case 3:
                    _mainUI.ShowPopup(_phrases[Random.Range(0,_phrases.Length)]);
                    _usable.SetActive(false);
                    break;
                default:
                    _mainUI.ShowPopup(_phrases[Random.Range(0,_phrases.Length)]);
                    break;
            }
            yield return new WaitUntil(() => !marker.activeSelf);
        }
    }
}
