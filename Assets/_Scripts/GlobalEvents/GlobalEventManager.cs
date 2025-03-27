using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GlobalEventManager : MonoBehaviour
{
    private float _updateInterval = 120f; // 2 минуты
    private MainUI _mainUI;
    private List<GlobalEventResponse> _cachedEvents = new();
    [SerializeField] private GlobalEventStarter _starter;

    [Inject]
    private void Construct(MainUI mainUI)
    {
        _mainUI = mainUI;
    }
    
    private void Start()
    {
        LoadCachedEvents();
        StartCoroutine(UpdateEventsRoutine());
    }

    private IEnumerator UpdateEventsRoutine()
    {
        while (true)
        {
            UpdateEvents();
            yield return new WaitForSeconds(_updateInterval);
        }
    }

    private async void UpdateEvents()
    {
        try
        {
            var events = await APIManager.Instance.GetEvents();
            _cachedEvents = events;
            SaveCachedEvents();
            ProcessEvents(events);
            Debug.Log("Ивенты успешно обновлены.");
        }
        catch (Exception e)
        {
            Debug.LogError("Ошибка при получении ивентов: " + e.Message);
            ProcessEvents(_cachedEvents);
            ShowErrorNotification();
        }
    }

    private void ProcessEvents(List<GlobalEventResponse> events)
    {
        foreach (var eventResponse in events)
        {
            CheckNotifications(eventResponse);
            ApplyEventEffects(eventResponse);
            UpdateUIForEvent(eventResponse);
        }
    }

    private DateTime ParseEventTime(string startDateTime)
    {
        return DateTime.Parse(startDateTime).ToLocalTime();
    }

    private TimeSpan TimeUntilEvent(DateTime eventStart)
    {
        return eventStart - DateTime.Now;
    }

    private DateTime GetNextEventStartTime(GlobalEventResponse eventResponse)
    {
        var startTime = DateTime.Parse(eventResponse.start_date_time).ToLocalTime();
        var now = DateTime.Now;
        var period = TimeSpan.FromHours(eventResponse.once_in_hours);

        if (now < startTime)
        {
            return startTime;
        }

        var elapsed = now - startTime;
        var periodsPassed = (int)Math.Ceiling(elapsed.TotalHours / eventResponse.once_in_hours);
        var nextStart = startTime.AddHours(periodsPassed * eventResponse.once_in_hours);

        return nextStart;
    }
    
    private bool IsEventActive(GlobalEventResponse eventResponse)
    {
        var nextStart = GetNextEventStartTime(eventResponse);
        var eventEnd = nextStart.AddMinutes(eventResponse.duration_in_minutes);
        var now = DateTime.Now;

        if (now >= nextStart && now <= eventEnd)
        {
            return true;
        }

        var previousStart = nextStart.AddHours(-eventResponse.once_in_hours);
        var previousEnd = previousStart.AddMinutes(eventResponse.duration_in_minutes);
        if (now >= previousStart && now <= previousEnd)
        {
            return true;
        }

        return false;
    }

    private void CheckNotifications(GlobalEventResponse eventResponse)
    {
        var nextStart = GetNextEventStartTime(eventResponse);
        var timeUntilEvent = nextStart - DateTime.Now;

        if (eventResponse.once_in_hours >= 168)
        {
            if (timeUntilEvent.TotalDays <= 1 && timeUntilEvent.TotalDays > 0)
            {
                ShowNotification(eventResponse, "Ивент начнется через день!");
            }
        }
        else
        {
            if (timeUntilEvent.TotalMinutes <= 15 && timeUntilEvent.TotalMinutes > 0)
            {
                ShowNotification(eventResponse, "Скоро начнется");
            }
        }
    }

    private void ShowNotification(GlobalEventResponse eventResponse, string message)
    {
        string notificationText = $"{message} {eventResponse.name}";
        _mainUI.ShowPopup(notificationText);
        Debug.Log(notificationText);
    }

    private void ApplyEventEffects(GlobalEventResponse eventResponse)
    {
        if (IsEventActive(eventResponse))
        {
            switch (eventResponse.name)
            {
                case "Light of":
                    _starter.StartLightEvent();
                    Debug.Log("Золото увеличено в 2 раза!");
                    break;
                case "Low price":
                    _starter.StartFoodEvent();
                    Debug.Log("Опыт увеличен в 2 раза!");
                    break;
                default:
                    Debug.LogWarning($"Неизвестный ивент: {eventResponse.name}");
                    break;
            }
        }
        else
        {
            ResetEventEffects(eventResponse);
        }
    }

    private void ResetEventEffects(GlobalEventResponse eventResponse)
    {
        switch (eventResponse.name)
        {
            case "Light of":
                _starter.StopLightEvent();
                break;
            case "Low price":
                _starter.StopFoodEvent();
                break;
        }
    }

    private void UpdateUIForEvent(GlobalEventResponse eventResponse)
    {
        if (IsEventActive(eventResponse))
        {
            string uiText = $"Активен ивент: {eventResponse.name} - Осталось: {TimeUntilEventEnd(eventResponse):hh\\:mm\\:ss}";
            Debug.Log(uiText);
        }
    }

    private TimeSpan TimeUntilEventEnd(GlobalEventResponse eventResponse)
    {
        var eventStart = ParseEventTime(eventResponse.start_date_time);
        var eventEnd = eventStart.AddMinutes(eventResponse.duration_in_minutes);
        return eventEnd - DateTime.Now;
    }

    private void ShowErrorNotification()
    {
        Debug.Log("Нет соединения с сервером. Используются последние данные.");
    }

    private void SaveCachedEvents()
    {
        string json = JsonUtility.ToJson(new SerializableEventList { events = _cachedEvents });
        PlayerPrefs.SetString("CachedEvents", json);
        PlayerPrefs.Save();
    }

    private void LoadCachedEvents()
    {
        string json = PlayerPrefs.GetString("CachedEvents", "");
        if (!string.IsNullOrEmpty(json))
        {
            var eventList = JsonUtility.FromJson<SerializableEventList>(json);
            _cachedEvents = eventList.events;
        }
    }
}

[Serializable]
public class SerializableEventList
{
    public List<GlobalEventResponse> events;
}