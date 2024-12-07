using System.Collections.Generic;
using UnityEngine;

public class ShopBase : MonoBehaviour
{
    [SerializeField] private List<ToolBase> _shopTools = new();
    [SerializeField] private bool _needToActivate;
    [SerializeField] private List<GameObject> _tools = new();
    
    private List<ToolBase> _activeTools = new();
    private List<ToolBase> _activeToolsToAdd = new();
    private List<ToolBase> _activeToolsToRemove = new();

    public async void InitializePlayerShop()
    {
        var shop = await APIManager.Instance.GetShop(Bootstrap.Instance.PlayerName);

        if (shop != null)
        {
            int index = 0;
            foreach (var tool in shop)
            {
                if (tool.Value == 0)
                {
                    print("FindValue");
                    var toolToAdd = _shopTools[tool.Value];
                    _activeTools.Add(toolToAdd);
                    toolToAdd.Buyed();
                    if (_needToActivate)
                    {
                        _tools[index].SetActive(true);
                        print("Activate");
                    }
                }
                index++;
            }
        }
        TimeLines.OnDayEnd += OnDayEnd;
    }

    private void OnDayEnd()
    {
        if (_activeToolsToAdd.Count > 0)
        {
            foreach (var tool in _activeToolsToAdd)
            {
                _tools[tool.Index].SetActive(true);
                _activeTools.Add(tool);
            }
            
            _activeToolsToAdd.Clear();
        }

        if (_activeToolsToRemove.Count <= 0) return;
        {
            foreach (var tool in _activeToolsToRemove)
            {
                _activeTools.Remove(tool);
                _tools[tool.Index].SetActive(false);
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
            PlayerData.ChangeCoins(price, false);
            APIManager.Instance.ChangeShop(playerName, shop);
            tool.Buyed();
        }
        
        _activeToolsToAdd.Add(tool);
    }

    public async void SellTool(string toolToSell)
    {
        var playerName = Bootstrap.Instance.PlayerName;
        var tool = GetTool(toolToSell);
        var price = tool.Price;
        var shop = await APIManager.Instance.GetShop(playerName);
        
        if (shop[toolToSell] == 1) return;
        
        shop[toolToSell] = 1;
        PlayerData.ChangeCoins(price);
        APIManager.Instance.ChangeShop(playerName, shop);
        tool.Sold();
        
        _activeToolsToRemove.Add(tool);
    }

    private ToolBase GetTool(string toolName)
    {
        foreach (var tool in _shopTools)
        {
            if (tool.ToolName == toolName)
                return tool;
        }

        return null;
    }
}