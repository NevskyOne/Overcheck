
public class StoryNPC : NPCBase
{
    public override void Setup(NPCData npcData, NPCService npcService)
    {
        SetDialog(npcData.Config);
    }
}
