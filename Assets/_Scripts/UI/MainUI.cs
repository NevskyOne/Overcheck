using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("Main menues")] 
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _sleepMenu;
    [SerializeField] private GameObject _holdingMenu;
    [Header("Cursors")] 
    [SerializeField] private GameObject _cursor;
    [SerializeField] private Image _cursorImg;
    [SerializeField] private Sprite[] _cursors;
    [Header("Additional UI")] 
    [SerializeField] private Slider _runSlider;
    [SerializeField] private Slider _saturationSlider;
    [SerializeField] private Slider _eventSlider;
    [SerializeField] private TMP_Text _eventTimer;
    [SerializeField] private GameObject _popupMenu;
    [SerializeField] private TMP_Text _popupText;

    private Coroutine _drainRunRoutine, _fillRunRoutine,
        _drainSaturationRoutine, _fillSaturationRoutine,
        _drainEventRoutine, _drainTimerRoutine;

    public void Pause() => _pauseMenu.SetActive(true);
    public void Sleep() => _sleepMenu.SetActive(true);
    public void Hold() => _holdingMenu.SetActive(true);

    public void CloseMenus()
    {
        _sleepMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _holdingMenu.SetActive(false);
    }

    public void ChangeCursor(int index) => _cursorImg.sprite = _cursors[index];
    public void ShowCursor() => _cursor.SetActive(true);
    public void HideCursor() => _cursor.SetActive(false);
    
    public void ShowPopup(string text)
    {
        _popupMenu.SetActive(true);
        _popupText.text = text;
    }
    public void HidePopup() => _popupMenu.SetActive(false);

    public void DrainRun()
    {
        StopCoroutine(_fillRunRoutine);
        _drainRunRoutine = StartCoroutine(SmoothSlideRoutine(_runSlider, 0));
    }

    public void FillRun()
    {
        StopCoroutine(_drainRunRoutine);
        _fillRunRoutine = StartCoroutine(SmoothSlideRoutine(_runSlider, 1));
    }

    public void DrainSaturation(float value)
    {
        StopCoroutine(_fillSaturationRoutine);
        _drainSaturationRoutine = StartCoroutine(SmoothSlideRoutine(_saturationSlider, value));
    }

    public void FillSaturation()
    {
        StopCoroutine(_drainSaturationRoutine);
        _fillSaturationRoutine = StartCoroutine(SmoothSlideRoutine(_saturationSlider, 1));
    }
    
    public void DrainEvent()
    {
        _drainEventRoutine = StartCoroutine(SmoothSlideRoutine(_eventSlider, 0));
        _drainTimerRoutine = StartCoroutine(TimerRoutine(_eventTimer, 180));
    }

    public void StopTimer()
    {
        StopCoroutine(_drainEventRoutine);
        StopCoroutine(_drainTimerRoutine);
        _eventSlider.value = 1;
    }
    
    public IEnumerator SmoothSlideRoutine(Slider slider, float endValue)
    {
        if (endValue > slider.value)
        {
            while (endValue > slider.value)
            {
                yield return new WaitForFixedUpdate();
                slider.value += Time.fixedDeltaTime;
            }
        }
        else
        {
            while (endValue < slider.value)
            {
                yield return new WaitForFixedUpdate();
                slider.value -= Time.fixedDeltaTime;
            }
        }
    } 
    
    public IEnumerator TimerRoutine(TMP_Text tmpText, int duration)
    {
        while (duration > 0)
        {
            yield return new WaitForSeconds(1);
            duration -= 1;
            tmpText.text = "${duration // 60}:${duration % 60}";
        }
    } 
}
