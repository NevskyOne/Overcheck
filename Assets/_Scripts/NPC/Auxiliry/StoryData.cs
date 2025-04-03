using UnityEngine;

[CreateAssetMenu(fileName = "StoryConfig", menuName = "Scriptable/StoryConfig")]
public class StoryData: ScriptableObject
{
    [SerializeField] private int _prefab;
    [SerializeField] private DialogConfig _config;
    [SerializeField] private SpawnEvent _spawnEvent;

    public int Prefab => _prefab;
    public DialogConfig Config => _config;
    public SpawnEvent SpawnEvent => _spawnEvent;
}

public enum SpawnEvent{
    AtDayStart, AtDayEnd, AtRandomTime
}