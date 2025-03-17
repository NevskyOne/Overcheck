using System;
using UnityEngine.Events;

[Serializable]
public class ConditionalEvent
{
    public UnityEvent Event;
    public UnityEvent Condition;
    
    public void ExecuteIf(DayData previousDay)
    {
        if (previousDay.InvokedEvents.Contains(Condition.GetPersistentMethodName(0)))
        {
            Event?.Invoke();
        }
    }
}   