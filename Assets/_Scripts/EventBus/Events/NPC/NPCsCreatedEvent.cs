using System.Collections.Generic;

public class NPCsCreatedEvent
{
    public readonly List<NPCData> NPCsCreated = new();

    public NPCsCreatedEvent(List<NPCData> npcsCreated)
    {
        NPCsCreated = npcsCreated;
    }
}