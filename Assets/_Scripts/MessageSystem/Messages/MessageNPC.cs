using UnityEngine;

[CreateAssetMenu(fileName = "NPCMessage", menuName = "Messages/NPCMessage")]
public class MessageNPC : Message
{
    [SerializeField] private StoryNPC _npc;
    [SerializeField] private DialogConfig _config;
    [SerializeField] private SpawnEvent _spawnEvent;
    
    public NPCBase StoryNPC => _npc;
    public DialogConfig Config => _config;
    public SpawnEvent SpawnEvent => _spawnEvent;
}

public enum SpawnEvent {AtDayStart, AtDayEnd, AtRandomTime}