using System;

[Serializable]
public class NPCData
{
    private NPCBase _prefab;
    private DocsData _docsData;
    private NPCAppearanceData _npcAppearanceData;
    private DialogConfig _config; 

    public DocsData DocsData => _docsData;
    public NPCAppearanceData NpcAppearanceData => _npcAppearanceData;
    public DialogConfig Config => _config;

    public void Setup(NPCBase prefab, DocsData docsData, NPCAppearanceData npcAppearanceData, DialogConfig config)
    {
        _prefab = prefab;
        _docsData = docsData;
        _npcAppearanceData = npcAppearanceData;
        _config = config;
    }
}
