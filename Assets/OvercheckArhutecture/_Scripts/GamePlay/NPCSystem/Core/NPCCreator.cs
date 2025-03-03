public class NPCCreator
{
    private NPCAppearanceRandomizer _appearanceRandomizer;
    private DocsRandomizer _docsRandomizer;
    
    public NPCCreator()
    {
        _appearanceRandomizer = new NPCAppearanceRandomizer();
        _docsRandomizer = new DocsRandomizer();
    }

    public NPCData CreateNPC()
    {
        var appearance = _appearanceRandomizer.Randomize();
        var docs = _docsRandomizer.Randomize();
        var data = new NPCData();
        data.Setup(docs, appearance);
        return data;
    }
}