using UnityEngine;

public class APIController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            APIManager.Instance.GetCoins("Almik");
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            APIManager.Instance.ChangeCoins("Almik", 20);
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            APIManager.Instance.ChangeCoins("Almik", 10);
        }
    }
}