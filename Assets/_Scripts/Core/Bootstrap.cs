using TMPro;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameObject _authorizationPanel;
    [SerializeField] private TMP_InputField _authorizationInputField;
    
    public static Bootstrap Instance { get; private set; }
    public bool IsAuthorizated => _isAuthorizated;
    public string PlayerName => _playerName;

    private bool _isAuthorizated;
    [SerializeField] private string _playerName;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        //TryGetPlayerName();
    }

    private async void TryGetPlayerName()
    {
        var playerName = PlayerPrefs.GetString(Constants.PLAYER_NAME_PLAYERPREFS_KEY);
        if (string.IsNullOrEmpty(playerName))
            _authorizationPanel.SetActive(true);
        else
        {
            await APIManager.Instance.Authorization(playerName);
            _isAuthorizated = true;
            _playerName = playerName;
        }
    }

    public async void Auth()
    {
        var playerName = _authorizationInputField.text;
        PlayerPrefs.SetString(Constants.PLAYER_NAME_PLAYERPREFS_KEY, playerName);
        await APIManager.Instance.Authorization(playerName);
        _isAuthorizated = true;
        _playerName = playerName;
    }
}