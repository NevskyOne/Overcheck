
public class StoryNPCData : NPCData
{
    private StoryNPC _prefab;
    public StoryNPC Prefab => _prefab;
    
    public void Setup(DocsData docsData, NPCAppearanceData npcAppearanceData, DialogConfig config, StoryNPC prefab)
    {
        base.Setup(docsData, npcAppearanceData, config);
        _prefab = prefab;
    }
}
