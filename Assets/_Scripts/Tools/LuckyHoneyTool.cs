

public class LuckyHoneyTool : Tool
{
    private void OnEnable() => FindFirstObjectByType<NPCManager>().EventChance = 0;
}
