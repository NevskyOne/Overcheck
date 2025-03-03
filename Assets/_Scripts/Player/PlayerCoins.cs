using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCoins
{
    private List<TMP_Text> _honeyCombsTextes;

    public static int HoneyCombs { get; private set; }

    private static event Action _onMoneyChange;

    public PlayerCoins(List<TMP_Text> honeyCombsText)
    {
        _honeyCombsTextes = honeyCombsText;
        
        UpdateUI();
        _onMoneyChange += UpdateUI;
    }

    public static async void InitializeCoins()
    {
        HoneyCombs = await APIManager.Instance.GetCoins(AuthBootstrap.Instance.PlayerName);
        _onMoneyChange?.Invoke();
    }

    public static async void ChangeCoins(int value, bool add = true)
    {
        var _serverValue = await APIManager.Instance.GetCoins(AuthBootstrap.Instance.PlayerName);
        HoneyCombs = Mathf.Clamp(add? _serverValue + value : _serverValue - value, 0, 1000000000);
        
        APIManager.Instance.ChangeCoins(AuthBootstrap.Instance.PlayerName, HoneyCombs);
        _onMoneyChange?.Invoke();
    }

    private void UpdateUI()
    {
        foreach (var text in _honeyCombsTextes)
        {
            text.text = PlayerCoins.HoneyCombs.ToString();
        }
    }
}
