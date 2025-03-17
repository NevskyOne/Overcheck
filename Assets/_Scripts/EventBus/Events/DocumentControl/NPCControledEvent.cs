public class NPCControledEvent
{
    public readonly NPCData NpcData;
    public readonly bool IsApproved;

    public NPCControledEvent(NPCData npcData, bool isApproved)
    {
        NpcData = npcData;
        IsApproved = isApproved;
    }
}