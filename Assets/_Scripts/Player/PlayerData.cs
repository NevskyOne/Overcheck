using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyCount;
    
    private int _coins;

    public void ChageCoins(int value, bool add = true)
    {
        _coins = Mathf.Clamp(add? _coins + value : _coins - value, 0, 1000000000);
        _moneyCount.text = _coins.ToString();
    }
}
