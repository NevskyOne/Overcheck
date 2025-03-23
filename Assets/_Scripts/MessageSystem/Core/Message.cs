using UnityEngine;

public abstract class Message : ScriptableObject
{
    public virtual void Invoke(EventBus eventBus)
    {
        eventBus.Invoke(new MessageInvokeEvent(this));
    }
}