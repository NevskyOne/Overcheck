using System.Collections.Generic;
using UnityEngine;

public class NPCCreator
{
    private NPCAppearanceRandomizer _appearanceRandomizer;
    private DocsRandomizer _docsRandomizer;
    
    public NPCCreator(AppearRandomStruct appearStruct)
    {
        _appearanceRandomizer = new NPCAppearanceRandomizer(appearStruct);
        _docsRandomizer = new DocsRandomizer();
    }

    public NPCData CreateNPC()
    {
        bool male = Random.Range(0, 2) == 1;
        List<DialogConfig> configs = RandomParamStruct.GeneralConfigs;
        configs.AddRange(male? RandomParamStruct.MaleConfigs : RandomParamStruct.FemaleConfigs);
        
        var appearance = _appearanceRandomizer.Randomize(male);
        var docs = _docsRandomizer.Randomize(male, appearance.Photo);
        var data = new NPCData();
        data.Setup(docs, appearance,configs[Random.Range(0, configs.Count)]);
        return data;
    }
}