using System.Collections.Generic;
using UnityEngine;

public class ShopBase : MonoBehaviour
{
    [SerializeField] private List<ToolBase> _tools = new();

    private List<ToolBase> _activeTools = new();
    private List<ToolBase> _activeToolsToAdd = new();
    private List<ToolBase> _activeToolsToRemove = new();
    
    private void Start()
    {
        TimeLines.OnDayEnd += OnDayEnd;
        InitializePlayerShop();
    }

    private async void InitializePlayerShop()
    {
        await APIManager.Instance.GetShop(Bootstrap.Instance.PlayerName);
    }

    private void OnDayEnd()
    {
        if (_activeToolsToAdd.Count > 0)
        {
            foreach (var tool in _activeToolsToAdd)
            {
                var toolToAdd = Instantiate(tool);
                _activeTools.Add(toolToAdd);
            }
            
            _activeToolsToAdd.Clear();
        }

        if (_activeToolsToRemove.Count <= 0) return;
        {
            foreach (var tool in _activeToolsToRemove)
            {
                var toolToRemove = _activeTools.Find(tool => true);
                _activeTools.Remove(toolToRemove);
                Destroy(toolToRemove.gameObject);
            }

            _activeToolsToRemove.Clear();
        }
    }

    public async void BuyTool(string toolToBuy)
    {
        var playerName = Bootstrap.Instance.PlayerName;
        var coins = await APIManager.Instance.GetCoins(playerName);
        var tool = GetTool(toolToBuy);
        var price = tool.Price;
        var shop = await APIManager.Instance.GetShop(playerName);

        if (shop[toolToBuy] == 0) return;
        
        if (coins >= price)
        {
            shop[toolToBuy] = 0;
            APIManager.Instance.ChangeCoins(playerName, coins - price);
            APIManager.Instance.ChangeShop(playerName, shop);
        }
        
        _activeToolsToAdd.Add(tool);
    }

    public async void SellTool(string toolToSell)
    {
        var playerName = Bootstrap.Instance.PlayerName;
        var coins = await APIManager.Instance.GetCoins(playerName);
        var tool = GetTool(toolToSell);
        var price = tool.Price;
        var shop = await APIManager.Instance.GetShop(playerName);
        
        if (shop[toolToSell] == 1) return;
        
        shop[toolToSell] = 1;
        APIManager.Instance.ChangeCoins(playerName, coins + price);
        APIManager.Instance.ChangeShop(playerName, shop);
        
        _activeToolsToRemove.Add(tool);
    }

    private ToolBase GetTool(string toolName)
    {
        foreach (var tool in _tools)
        {
            if (tool.ToolName == toolName)
                return tool;
        }

        return null;
    }
}