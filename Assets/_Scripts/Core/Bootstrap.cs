using System;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameObject _authorizationPanel;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private TMP_InputField _authorizationInputField;
    [SerializeField] private TMP_Text _playerText;
    
    public static Bootstrap Instance { get; private set; }
    public string PlayerName => _playerName;

    private ShopBase _playerShopBase;
    private bool _isAuthorizated;
    private string _playerName;
    
    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        APIManager.Instance.OnAuthEnd += OnAuthCompleted;
        SceneManager.activeSceneChanged += OnActiveSceneChanged; 
        
        TryGetPlayerName();
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name == Constants.GAMEPLAY_SCENE_NAME)
        {
            _playerShopBase = FindFirstObjectByType<ShopBase>();
            _playerShopBase.InitializePlayerShop();
        }
    }

    private async void TryGetPlayerName()
    {
        try
        {
            var playerName = PlayerPrefs.GetString(Constants.PLAYER_NAME_PLAYERPREFS_KEY);
            if (string.IsNullOrEmpty(playerName))
            {
                _authorizationPanel.SetActive(true);
                _menuPanel.SetActive(false);
            }
            else
            {
                await APIManager.Instance.Authorization(playerName);
                _playerText.text = _playerName;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void OnAuthCompleted(AuthorizationResponse response)
    {
        _isAuthorizated = true;
        _playerName = response.Name;
        _authorizationPanel.SetActive(false);
        _menuPanel.SetActive(true);
    }

    public async void Auth()
    {
        try
        {
            var playerName = _authorizationInputField.text;
            PlayerPrefs.SetString(Constants.PLAYER_NAME_PLAYERPREFS_KEY, playerName);
            await APIManager.Instance.Authorization(playerName);
            
            _playerText.text = _playerName;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        PlayerPrefs.Save();
    }
}