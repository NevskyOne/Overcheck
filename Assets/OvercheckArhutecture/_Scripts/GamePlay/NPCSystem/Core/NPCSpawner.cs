using UnityEngine;
using Zenject;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private NPCBase _npcBasePrefab;
    
    private EventBus _eventBus;
    private bool _isGameStarted;

    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _eventBus = eventBus;
        
        _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        _eventBus.Subscribe<GamePausedEvent>(OnGamePaused);
        _eventBus.Subscribe<SpawnNPCRequestEvent>(SpawnNPC);
    }

    private void OnGameStarted(GameStartedEvent e)
    {
        _isGameStarted = true;
    }

    private void OnGamePaused(GamePausedEvent e)
    {
        _isGameStarted = false;
    }
    
    public void SpawnNPC(SpawnNPCRequestEvent e)
    {
        if (!_isGameStarted) return;

        var spawnData = e.NPCSpawnData;
        var newNpc = Instantiate(_npcBasePrefab, spawnData.SpawnPosition, Quaternion.identity);
        newNpc.Setup(spawnData.NPCData);
        _eventBus.Invoke(new NPCSpawnedEvent(spawnData, newNpc));
    }
}