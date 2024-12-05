
public class PiggyBankTool : Tool
{
    private void OnEnable() => TimeLines.MoneyFactor = 1.1f;
    private void OnDisable() => TimeLines.MoneyFactor = 1f;
}
