using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCoins
{
    private static List<TMP_Text> _honeyCombsTextes;
    private static TMP_Text _dayCoinsText;
    
    public static int HoneyCombs { get; private set; }
    private static int _dayCoins;
    

    public PlayerCoins(List<TMP_Text> honeyCombsText, TMP_Text dayCoinsText, EventBus eventBus)
    {
        _honeyCombsTextes = honeyCombsText;
        _dayCoinsText = dayCoinsText;
        UpdateUI();
        
        eventBus.Subscribe((NPCControledEvent e) =>
        {
            _dayCoins += e.IsApproved != e.NpcData.DocsData.Fake ? 1 : -1;
        });
        eventBus.Subscribe((AllDayNPCsEndedEvent _) =>
        {
            ChangeCoins(_dayCoins);
        });
    }

    public static async void InitializeCoins()
    {
        HoneyCombs = await APIManager.Instance.GetCoins(AuthBootstrap.Instance.PlayerName);
        UpdateUI();
    }

    public static async void ChangeCoins(int value)
    {
        var _serverValue = await APIManager.Instance.GetCoins(AuthBootstrap.Instance.PlayerName);
        HoneyCombs = Mathf.Clamp(_serverValue + value, 0, 1000000000);
        
        APIManager.Instance.ChangeCoins(AuthBootstrap.Instance.PlayerName, HoneyCombs);
        UpdateUI();
    }

    private static void UpdateUI()
    {
        _dayCoinsText.text = _dayCoins.ToString();
        foreach (var text in _honeyCombsTextes)
        {
            text.text = PlayerCoins.HoneyCombs.ToString();
        }
    }
}
