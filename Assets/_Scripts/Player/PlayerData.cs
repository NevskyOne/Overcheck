using _Scripts.UI;
using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private TMP_Text _honeyCombsText;

    private int _honeyCombs;

    public async void ChageCoins(int value, bool add = true)
    {
        var _serverValue = await APIManager.Instance.GetCoins(Bootstrap.Instance.PlayerName);
        _honeyCombs = Mathf.Clamp(add? _serverValue + value : _serverValue - value, 0, 1000000000);
        
        APIManager.Instance.ChangeCoins(SettingsUI.PlayerName, _honeyCombs);
    }
}
