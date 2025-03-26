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

    public override void Invoke(EventBus eventBus)
    {
        base.Invoke(eventBus);
        var storyData = new StoryNPCData();
        storyData.Setup(new DocsData(), new NPCAppearanceData(), _config, _npc);
        eventBus.Invoke(new SpawnNPCRequestEvent(new NPCSpawnData(storyData)));
    }
}

public enum SpawnEvent {AtDayStart, AtDayEnd, AtRandomTime}