using UnityEngine;
using Zenject;

public class MessageService : MonoBehaviour
{
    [SerializeField] private Phone _phone;

    private EventBus _eventBus;
    
    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _eventBus = eventBus;
        
        _eventBus.Subscribe<MessageInvokeEvent>(OnMessageInvoke);
    }

    private void OnMessageInvoke(MessageInvokeEvent e)
    {
        if (e.Message is CallMessage message)
            _phone.CallMessage(message);
    }
}