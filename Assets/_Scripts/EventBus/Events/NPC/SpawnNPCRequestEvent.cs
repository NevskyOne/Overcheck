public class SpawnNPCRequestEvent
{
    public readonly NPCSpawnData NPCSpawnData;

    public SpawnNPCRequestEvent(NPCSpawnData npcSpawnData)
    {
        NPCSpawnData = npcSpawnData;
    }
}