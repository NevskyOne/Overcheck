using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainUI : MonoBehaviour
{
    [Header("Main menus")] 
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _sleepMenu;
    [SerializeField] private GameObject _holdingMenu;
    [SerializeField] private GameObject _tableMenu;
    [Header("Cursors")] 
    [SerializeField] private GameObject _cursor;
    [SerializeField] private Image _cursorImg;
    [SerializeField] private Sprite[] _cursors;
    [Header("Sliders")] 
    [SerializeField] private Slider _runSlider;
    [SerializeField] private Slider _saturationSlider;
    [SerializeField] private Slider _eventSlider;
    [Header("Additional UI")] 
    [SerializeField] private Image _saturationBack;
    [SerializeField] private Image _saturationFront;
    [SerializeField] private TMP_Text _eventTimer;
    [Header("Popup")] 
    [SerializeField] private Animator _popupAnim;
    [SerializeField] private TMP_Text _popupText;
    
    
    private Coroutine _drainRunRoutine, _fillRunRoutine,
        _drainSaturationRoutine, _fillSaturationRoutine,
        _drainEventRoutine, _drainTimerRoutine;
    
    [Inject]
    private void Initialize(EventBus eventBus)
    {
        eventBus.Subscribe<AllDayNPCsEndedEvent>(_ => ShowPopup("Рабочий день окончен!"));
    }
    
    public void Pause() => _pauseMenu.SetActive(true);
    public void Unpause() => Player.Interactions.PauseGame();
    public void Sleep() => _sleepMenu.SetActive(true);
    public void Hold() => _holdingMenu.SetActive(true);

    public void CloseMenus()
    {
        _sleepMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _holdingMenu.SetActive(false);
        _tableMenu.SetActive(false);
    }

    public void ChangeCursor(int index) => _cursorImg.sprite = _cursors[index];

    public void ChangeSaturColor(Color color)
    {
        _saturationBack.color = color;
        _saturationFront.color = color;
    }

    public void ShowCursor()
    {
        _cursor.SetActive(true);
        _runSlider.gameObject.SetActive(true);
    }

    public void HideCursor(){
        _cursor.SetActive(false);
        _runSlider.gameObject.SetActive(false);
    }
    
    public void ShowPopup(string text)
    {
        _popupAnim.Play("PopupShow");
        _popupText.text = text;
    }
    public void HidePopup() => _popupAnim.Play("PopupHide");

    public void DrainRun()
    {
        if(_fillRunRoutine != null) StopCoroutine(_fillRunRoutine);
        _drainRunRoutine = StartCoroutine(SmoothSlideRoutine(_runSlider, 0, 1, true));
    }

    public void FillRun()
    {
        if(_drainRunRoutine != null) StopCoroutine(_drainRunRoutine);
        _fillRunRoutine = StartCoroutine(SmoothSlideRoutine(_runSlider, _runSlider.maxValue, 0.3f));
    }

    public void DrainSaturation(float value)
    {
        if(_fillSaturationRoutine != null) StopCoroutine(_fillSaturationRoutine);
        _drainSaturationRoutine = StartCoroutine(SmoothSlideRoutine(_saturationSlider, value));
    }

    public void FillSaturation(float value)
    {
        if(_drainSaturationRoutine != null) StopCoroutine(_drainSaturationRoutine);
        _fillSaturationRoutine = StartCoroutine(SmoothSlideRoutine(_saturationSlider, value));
    }
    
    public void DrainEvent(int duration)
    {
        if(_drainEventRoutine != null) StopCoroutine(_drainEventRoutine);
        if(_drainTimerRoutine != null) StopCoroutine(_drainTimerRoutine);
        _eventSlider.maxValue = duration;
        _eventSlider.value = duration;
        _eventSlider.gameObject.SetActive(true);
        _eventTimer.gameObject.SetActive(true);
        _drainEventRoutine = StartCoroutine(SmoothSlideRoutine(_eventSlider, 0));
        _drainTimerRoutine = StartCoroutine(TimerRoutine(_eventTimer, duration));
    }

    public void StopEvent()
    {
        _eventSlider.gameObject.SetActive(false);
        _eventTimer.gameObject.SetActive(false);
        StopCoroutine(_drainEventRoutine);
        StopCoroutine(_drainTimerRoutine);
        _eventSlider.value = 1;
    }
    
    public IEnumerator SmoothSlideRoutine(Slider slider, float endValue, float multiplaier = 1, bool stopSprint = false)
    {
        if (endValue > slider.value)
        {
            while (endValue > slider.value)
            {
                yield return new WaitForFixedUpdate();
                slider.value += Time.fixedDeltaTime * multiplaier;
            }
        }
        else
        {
            while (endValue < slider.value)
            {
                yield return new WaitForFixedUpdate();
                slider.value -= Time.fixedDeltaTime * multiplaier;
            }
        }
        if(stopSprint) Player.Interactions.StopSprint();
    } 
    
    public IEnumerator TimerRoutine(TMP_Text tmpText, int duration)
    {
        while (duration > 0)
        {
            yield return new WaitForSeconds(1);
            duration -= 1;
            tmpText.text = $"{(int)(duration / 60)}:{duration % 60}";
        }
    } 
}
