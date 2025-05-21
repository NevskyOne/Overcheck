using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCoins
{
    private static List<TMP_Text> _honeyCombsTextes;
    private static TMP_Text _dayCoinsText;

    public static int HoneyCombs { get; private set; } = 30;
    private static int _dayCoins;
    

    public PlayerCoins(List<TMP_Text> honeyCombsText, TMP_Text dayCoinsText, EventBus eventBus)
    {
        _honeyCombsTextes = honeyCombsText;
        _dayCoinsText = dayCoinsText;
        UpdateUI();
        
        eventBus.Subscribe((NPCControledEvent e) =>
        {
            if(e.AddCoints)
                _dayCoins += e.IsApproved != e.NpcData.DocsData.Fake ? 1 : -1;
            Debug.Log(_dayCoins);
        });
        eventBus.Subscribe((AllDayNPCsEndedEvent _) =>
        {
            ChangeCoins(_dayCoins);
        });
    }
    
    public static  void ChangeCoins(int value)
    {
        HoneyCombs = Mathf.Clamp(HoneyCombs + value, 0, 1000000000);
        UpdateUI();
    }

    private static void UpdateUI()
    {
        _dayCoinsText.text = _dayCoins.ToString();
        foreach (var text in _honeyCombsTextes)
        {
            text.text = HoneyCombs.ToString();
        }
    }
}
