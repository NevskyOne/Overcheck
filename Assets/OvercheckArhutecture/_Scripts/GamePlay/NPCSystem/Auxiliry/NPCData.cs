using System;

[Serializable]
public class NPCData
{
    private DocsData _docsData;
    private NPCAppearanceData _npcAppearanceData;
    //Здесь еще какая-то иная дата, которая не касается внешности и документов

    public DocsData DocsData => _docsData;

    public NPCAppearanceData NpcAppearanceData => _npcAppearanceData;

    public void Setup(DocsData docsData, NPCAppearanceData npcAppearanceData)
    {
        _docsData = docsData;
        _npcAppearanceData = npcAppearanceData;
    }
}