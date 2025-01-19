public class FamousCorporateTool : Tool
{
    private NPCManager _npcManager => FindFirstObjectByType<NPCManager>();
    private void OnEnable()
    {
        for (var i = 0; i < _npcManager.NpcList.Count; i++)
        {
            var day = _npcManager.NpcList[i];
            _npcManager.NpcList[i] = new ()
            {
                NormalNPC = day.NormalNPC,
                EternityNPC = day.EternityNPC + 1,
                TutorialNPC = day.TutorialNPC
            };
        }

    }
}
