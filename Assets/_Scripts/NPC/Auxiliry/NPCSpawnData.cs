using UnityEngine;

public class NPCSpawnData
{
    public readonly NPCData NPCData;
    public readonly Vector3 SpawnPosition;

    public NPCSpawnData(NPCData npcData, Vector3 spawnPosition)
    {
        NPCData = npcData;
        SpawnPosition = spawnPosition;
    }
}