

public class LuckyHoneyTool : ToolBase
{
    private void OnEnable() => FindFirstObjectByType<NPCManager>().EventChance = 0;
}
