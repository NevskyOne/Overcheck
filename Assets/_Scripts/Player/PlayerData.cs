using System;
using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private TMP_Text _honeyCombsText;

    public static int HoneyCombs { get; private set; }

    private static event Action _onMoneyChange;

    private void Start()
    {
        _onMoneyChange += () =>
        {
            //_honeyCombsText.text = PlayerData.HoneyCombs.ToString();
        };
    }

    public static async void ChangeCoins(int value, bool add = true)
    {
        var _serverValue = await APIManager.Instance.GetCoins(Bootstrap.Instance.PlayerName);
        HoneyCombs = Mathf.Clamp(add? _serverValue + value : _serverValue - value, 0, 1000000000);
        
        APIManager.Instance.ChangeCoins(Bootstrap.Instance.PlayerName, HoneyCombs);
        _onMoneyChange?.Invoke();
    }
    
}
