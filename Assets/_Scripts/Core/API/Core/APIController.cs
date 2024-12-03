using UnityEngine;

public class APIController : MonoBehaviour
{
    [SerializeField] private ShopBase _shop;
    [SerializeField] private ToolBase _toolBase;
    
    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _shop.BuyTool(_toolBase.ToolName);
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            APIManager.Instance.ChangeCoins(Bootstrap.Instance.PlayerName, 100);
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            await APIManager.Instance.GetCoins(Bootstrap.Instance.PlayerName);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            await APIManager.Instance.Authorization(Bootstrap.Instance.PlayerName);
        }
    }
}