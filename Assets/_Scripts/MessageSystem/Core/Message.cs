using UnityEngine;

public abstract class Message : ScriptableObject
{
    public void Invoke(EventBus eventBus)
    {
        eventBus.Invoke(new MessageInvokeEvent(this));
    }
}