using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopBase : MonoBehaviour
{
    [SerializeField] private List<ToolBase> _shopTools = new();
    [SerializeField] private bool _needToActivate;
    [SerializeField] private List<Tool> _tools = new();
    
    private List<ToolBase> _activeTools = new();
    private List<ToolBase> _activeToolsToAdd = new();
    private List<ToolBase> _activeToolsToRemove = new();

    public async void InitializePlayerShop()
    {
        var shop = await APIManager.Instance.GetShop(Bootstrap.Instance.PlayerName);

        if (shop != null)
        {
            foreach (var tool in shop)
            {
                if (tool.Value == 0)
                {
                    var toolToAdd = _shopTools[tool.Value];
                    _activeTools.Add(toolToAdd);
                    toolToAdd.Buyed();
                    var key = tool.Key;
                    Debug.Log(key);
                    if (_needToActivate)
                    {
                        foreach (var lTool in _tools)
                        {
                            if (lTool.ToolName.ToString() != key) continue;
                            lTool.gameObject.SetActive(true);
                            break;
                        }
                        
                        //_tools[index].SetActive(true);
                    }
                }
            }
        }
        APIManager.Instance.ChangeCoins(Bootstrap.Instance.PlayerName, 1000);
        TimeLines.OnDayEnd += OnDayEnd;
    }

    private void OnDayEnd()
    {
        if (_activeToolsToAdd.Count > 0)
        {
            foreach (var tool in _activeToolsToAdd)
            {
                foreach (var lTool in _tools.Where(lTool => lTool.ToolName.ToString() == tool.ToolName))
                {
                    lTool.gameObject.SetActive(true);
                    break;
                }

                //_tools[tool.Index].SetActive(true);
                _activeTools.Add(tool);
            }
            
            _activeToolsToAdd.Clear();
        }

        if (_activeToolsToRemove.Count <= 0) return;
        {
            foreach (var tool in _activeToolsToRemove)
            {
                _activeTools.Remove(tool);

                foreach (var lTool in _tools.Where(lTool => lTool.ToolName.ToString() == tool.ToolName))
                {
                    lTool.gameObject.SetActive(true);
                    break;
                }

                //_tools[tool.Index].SetActive(false);
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
            APIManager.Instance.ChangeShop(playerName, shop);
            tool.Buyed();
            PlayerData.ChangeCoins(price, false);
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
        APIManager.Instance.ChangeShop(playerName, shop);
        tool.Sold();
        PlayerData.ChangeCoins(price);
        
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