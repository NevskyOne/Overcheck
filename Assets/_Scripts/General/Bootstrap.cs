using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private bool _isNewGame;
    
    private EventBus _eventBus;

    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _eventBus = eventBus;
        StartGame();
    }
    
    public void StartGame()
    {
        _eventBus.Invoke(new GameStartedEvent(_isNewGame));
    }
}