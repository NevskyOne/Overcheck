using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class DaysService : MonoBehaviour
{
    [SerializeField] private List<DayData> _days = new();

    private ISaver _saver;
    private EventBus _eventBus;
    public static int CurrentDay { get; private set; }

    [Inject]
    private void Initialize(ISaver saver, EventBus eventBus)
    {
        _saver = saver;
        _eventBus = eventBus;

        _eventBus.Subscribe<GameStartedEvent>(OnGameStart);
        _eventBus.Subscribe<EventHasBeenInvoked>(OnEventInvoked);
        _eventBus.Subscribe<NPCsCreatedEvent>(SetupNPCToDays);
        _eventBus.Subscribe<AutoSaveEvent>(OnAutoSave);
    }

    private void OnAutoSave(AutoSaveEvent obj)
    {
        _saver.Save(_days, SavePathConstants.DaysDataSavePath);
        _saver.Save(CurrentDay, SavePathConstants.CurrentDaySavePath);
    }

    private void OnGameStart(GameStartedEvent e)
    {
        var randomNpcCount = 0;

        foreach (var day in _days)
        {
            randomNpcCount += day.RandomNpcCount;
        }

        _eventBus.Invoke(new CreateNPCRequestEvent(randomNpcCount, e.IsNewGame));

        if (!e.IsNewGame)
        {
            CurrentDay = _saver.Load<int>(SavePathConstants.CurrentDaySavePath);
            var days = _saver.Load<List<DayData>>(SavePathConstants.DaysDataSavePath);
            for (var i = 0; i < days.Count; i++)
            {
                foreach (var invokedEvent in days[i].InvokedEvents)
                {
                    _days[i].InvokedEvents.Add(invokedEvent);
                }
            }
        }
    }

    private void StartNewDay(int day)
    {
        CurrentDay = day;
        var currentDayData = _days[CurrentDay];
        _eventBus.Invoke(new NewDayStartedEvent(currentDayData));
        StartCoroutine(DayCycleRoutine());
    }

    private void EndDay()
    {
        var currentDayData = _days[CurrentDay];
        _saver.Save(_days, SavePathConstants.DaysDataSavePath);
        _saver.Save(CurrentDay, SavePathConstants.CurrentDaySavePath);
        _eventBus.Invoke(new DayEndedEvent(currentDayData));
    }
    
    private IEnumerator DayCycleRoutine()
    {
        var currentDayData = _days[CurrentDay];
        yield return new WaitForSeconds(1f);
        for (var i = 0; i < currentDayData.Events.Count; i++)
        {
            currentDayData.Events[i].Invoke();
            yield return new WaitForSeconds(1f);
        }

        if (currentDayData.ConditionalEvents.Count > 0)
        {
            var previousDay = _days[CurrentDay - 1];
            for (var i = 0; i < currentDayData.ConditionalEvents.Count; i++)
            {
                currentDayData.ConditionalEvents[i].ExecuteIf(previousDay);
                yield return new WaitForSeconds(1f);
            }
        }

        EndDay();
    }

    private void SetupNPCToDays(NPCsCreatedEvent e)
    {
        var npcList = e.NPCsCreated;
        var index = 0;

        foreach (var day in _days)
        {
            index += day.RandomNpcCount;
            for (var j = index - day.RandomNpcCount; j < index; j++)
            {
                day.NPCs.Add(npcList[j]);
            }
        }
        
        StartNewDay(0);
    }

    private void OnEventInvoked(EventHasBeenInvoked e)
    {
        _days[CurrentDay].InvokedEvents.Add(e.Action.Method.Name);
    }
}