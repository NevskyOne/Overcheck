public class NPCControledEvent
{
    public readonly NPCData NpcData;
    public readonly bool IsApproved;
    public readonly bool AddCoints;

    public NPCControledEvent(NPCData npcData, bool isApproved, bool addCoints)
    {
        NpcData = npcData;
        IsApproved = isApproved;
        AddCoints = addCoints;
    }
}