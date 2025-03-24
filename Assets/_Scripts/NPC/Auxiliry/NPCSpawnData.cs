using UnityEngine;

public class NPCSpawnData
{
    public readonly NPCData NPCData;
    public Vector3 SpawnPosition;

    public NPCSpawnData(NPCData npcData, Vector3 spawnPosition = default)
    {
        NPCData = npcData;
        SpawnPosition = spawnPosition;
    }
}