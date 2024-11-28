using _Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public static void LoadScene(int index) => SceneManager.LoadScene(index);

    public void StartGame(bool fromZero)
    {
        if (fromZero)
            SettingsUI.CurrentDay = 0;
        
        LoadScene(1);
    }

    public void QuitGame() => Application.Quit();
}
