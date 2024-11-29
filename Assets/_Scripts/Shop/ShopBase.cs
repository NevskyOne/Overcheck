using System.Collections.Generic;
using UnityEngine;

public class ShopBase : MonoBehaviour
{
    [SerializeField] private List<ToolBase> _tools = new();
    
    public async void BuyTool(ToolBase toolToBuy)
    {
        var playerName = Bootstrap.Instance.PlayerName;
        var coins = await APIManager.Instance.GetCoins(playerName);
        var price = GetPrice(toolToBuy.ToolName);
        var shop = await APIManager.Instance.GetShop(playerName);

        if (shop[toolToBuy.ToolName] == 0) return;
        
        if (coins >= price)
        {
            shop[toolToBuy.ToolName] = 0;
            APIManager.Instance.ChangeCoins(playerName, coins - price);
            APIManager.Instance.ChangeShop(playerName, shop);
        }
    }

    public void SellTool(ToolBase toolToSell)
    {
        
    }

    private int GetPrice(string toolName)
    {
        foreach (var tool in _tools)
        {
            if (tool.ToolName == toolName)
                return tool.Price;
        }

        return 0;
    }
}