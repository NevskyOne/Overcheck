public class NPCSpawnedEvent
{
    public readonly NPCSpawnData SpawnData;
    public readonly NPCBase NPC;

    public NPCSpawnedEvent(NPCSpawnData spawnData, NPCBase npc)
    {
        SpawnData = spawnData;
        NPC = npc;
    }
}