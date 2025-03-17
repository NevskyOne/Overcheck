using UnityEngine;
using Zenject;

public class RandomEventContainer : MonoBehaviour
{
    private EventBus _eventBus;
    
    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void RandomEvent()
    {
        // Какая-то логика
        _eventBus.Invoke(new EventHasBeenInvoked(RandomEvent));
    }
}