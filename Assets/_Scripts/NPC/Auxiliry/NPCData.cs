using System;

[Serializable]
public class NPCData
{
    

    private DocsData _docsData;
    private NPCAppearanceData _npcAppearanceData;
    private DialogConfig _config; 

    public DocsData DocsData => _docsData;
    public NPCAppearanceData NpcAppearanceData => _npcAppearanceData;
    public DialogConfig Config => _config;
    

    public virtual void Setup(DocsData docsData, NPCAppearanceData npcAppearanceData, DialogConfig config)
    {
        _docsData = docsData;
        _npcAppearanceData = npcAppearanceData;
        _config = config;
    }
}
