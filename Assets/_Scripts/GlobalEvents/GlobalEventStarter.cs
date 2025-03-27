using UnityEngine;

public class GlobalEventStarter : MonoBehaviour
{
    [SerializeField] private GameObject _light;
    
    public void StartLightEvent()
    {
        _light.SetActive(false);
    }

    public void StopLightEvent()
    {
        _light.SetActive(true);
    }

    public void StartFoodEvent()
    {
        ShopBase.ChangePrices(5, 10, 15);
    }

    public void StopFoodEvent()
    {
        ShopBase.ChangePrices(2, 5, 10);
    }
}