using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCCreator
{
    private NPCAppearanceRandomizer _appearanceRandomizer;
    private DocsRandomizer _docsRandomizer;
    private DocumentDataBase _documentDataBase;
    
    public NPCCreator(AppearRandomStruct appearStruct, DocumentDataBase docBase, Transform criminalParent)
    {
        _appearanceRandomizer = new NPCAppearanceRandomizer(appearStruct);
        _docsRandomizer = new DocsRandomizer(20,10, 6);
        _documentDataBase = docBase;

        var criminals = _docsRandomizer.CreateCriminals();
        var criminalsText = criminalParent.GetComponentsInChildren<TMP_Text>();
        for (int i = 0; i < criminals.Count; i++)
        {
            criminalsText[i].text = criminals[i];
        }
    }

    public NPCData CreateNPC()
    {
        bool male = Random.Range(0, 2) == 1;
        List<DialogConfig> configs = RandomParamStruct.GeneralConfigs;
        configs.AddRange(male? RandomParamStruct.MaleConfigs : RandomParamStruct.FemaleConfigs);
        
        var appearance = _appearanceRandomizer.Randomize(male);
        var docs = _docsRandomizer.Randomize(male, appearance.Photo, _documentDataBase);
        var data = new NPCData();
        data.Setup(0, docs, appearance,configs[Random.Range(0, configs.Count)]);
        return data;
    }
}