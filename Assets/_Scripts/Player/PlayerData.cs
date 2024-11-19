using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyCount;
    
    private uint _coins;

    public void ChageCoins(uint value, bool add = true)
    {
        _coins = add? _coins + value : _coins - value;
        _moneyCount.text = _coins.ToString();
    }
}
