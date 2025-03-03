using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NPCService : MonoBehaviour
{
    private NPCDataBaseService _npcDataBaseService;
    private NPCCreator _npcCreator;
    private EventBus _eventBus;
    private ISaver _saver;
    
    [Inject]
    private void Initialize(NPCDataBaseService npcDataBaseService, ISaver saver, EventBus eventBus)
    {
        _npcDataBaseService = npcDataBaseService;
        _saver = saver;
        _eventBus = eventBus;
        _npcCreator = new NPCCreator();
        
        _eventBus.Subscribe<CreateNPCRequestEvent>(LoadNpcDatas);
    }
    
    private void LoadNpcDatas(CreateNPCRequestEvent e)
    {
        if (e.IsNewGame)
            CreateNewNpcDatas(e.Count);
        else
            LoadSavedNpcDatas();

        var dataBase = _npcDataBaseService.GetDB();
        var npcList = (List<NPCData>)dataBase;
        _eventBus.Invoke(new NPCsCreatedEvent(npcList));
    }

    private void CreateNewNpcDatas(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var npc = _npcCreator.CreateNPC();
            _npcDataBaseService.AddToDB(npc);
        }
        
        var dataBase = _npcDataBaseService.GetDB();
        _saver.Save(dataBase, SavePathConstants.NPCDataBaseSavePath);
    }

    private void LoadSavedNpcDatas()
    {
        var npcs = _saver.Load<List<NPCData>>(SavePathConstants.NPCDataBaseSavePath);
        foreach (var npc in npcs)
        {
            _npcDataBaseService.AddToDB(npc);
        }
    }
}