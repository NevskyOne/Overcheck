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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }
    
    public void StartGame()
    {
        _eventBus.Invoke(new GameStartedEvent(_isNewGame));
    }
}