using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private List<NPCBase> _prefabs;
    [SerializeField] private Transform _defaultSpawn;
    private EventBus _eventBus;
    private NPCService _npcService;
    private bool _isGameStarted;
    
    [Inject] private DiContainer _container;

    [Inject]
    private void Initialize(EventBus eventBus, NPCService npcService)
    {
        _eventBus = eventBus;
        _npcService = npcService;
        
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
        if (spawnData is StoryNPCData)
        
        if (spawnData.SpawnPosition == default)
            spawnData.SpawnPosition = _defaultSpawn.position;
        var newNpc = _container.InstantiatePrefab(
            spawnData.NPCData.Prefab, spawnData.SpawnPosition, Quaternion.identity, null).GetComponent<NPCBase>();
        newNpc.Setup(spawnData.NPCData, _npcService);
        _eventBus.Invoke(new NPCSpawnedEvent(spawnData, newNpc));
    }
}