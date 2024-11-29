using UnityEngine;

public class APIController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            APIManager.Instance.Authorization("Almik");
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            APIManager.Instance.Authorization("DukFunduk");
        }
    }
}