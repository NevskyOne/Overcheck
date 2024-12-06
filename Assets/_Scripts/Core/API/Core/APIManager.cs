using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager
{
    public event Action<AuthorizationResponse> OnAuthEnd;
    
    private static APIManager _instance;
    public static APIManager Instance => _instance ??= new APIManager();
    
    private const string UUID = "a035437c-0e73-48c3-9d62-2da4ed3298eb";
    private const string COINS = "coins";
    private const string SHOP_NAME = "shop_base";
    
    private Dictionary<string, int> _currentShop = new();
    private bool _haveInternetConnection;
    private int _currentCoins;
    
    private APIManager()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            _haveInternetConnection = false;
        else
            _haveInternetConnection = true;
    }
    
    public async Task Authorization(string playerName)
    {
        if (_haveInternetConnection)
        {
            var getPlayersListRequestRaw = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/");
            var getPlayersListRequest = await SendRequest(getPlayersListRequestRaw);
            await SendLog("Проверка сущетсвует ли игрок. ", playerName, new Dictionary<string, int>());

            var response = JsonConvert.DeserializeObject<ObjectResponse[]>(getPlayersListRequest.downloadHandler.text);
            var playersList = new Dictionary<string, Dictionary<string, int>>();

            foreach (var player in response)
            {
                var name = player.name;
                var res = player.resources;
                playersList.Add(name, res);
            }

            if (!playersList.ContainsKey(playerName))
            {
                await RegisterPlayer(playerName);
                OnAuthEnd?.Invoke(new AuthorizationResponse(playerName, false));
                return;
            }

            OnAuthEnd?.Invoke(new AuthorizationResponse(playerName, true));
        }
        else
        {
            var name = PlayerPrefs.GetString(Constants.PLAYER_NAME_PLAYERPREFS_KEY);
            
            if (string.IsNullOrEmpty(name))
            {
                _currentShop = new Dictionary<string, int>
                {
                    { Constants.FACE_SCANNER, 1 },
                    { Constants.ANTI_CRIME_SYSTEM, 1 },
                    { Constants.PIGGY_BANK, 1 },
                    { Constants.HAPPY_HONEY, 1 },
                    { Constants.HONORARY_CORPORATION, 1 }
                };

                _currentCoins = 0;
                
                var serializedShop = JsonConvert.SerializeObject(_currentShop);
                PlayerPrefs.SetString(Constants.PLAYER_NAME_PLAYERPREFS_KEY, playerName);
                PlayerPrefs.SetString(SHOP_NAME, serializedShop);
                PlayerPrefs.SetInt(COINS, 1);
                PlayerPrefs.Save();
            }
            else
            {
                var serializedShop = PlayerPrefs.GetString(SHOP_NAME);
                _currentCoins = PlayerPrefs.GetInt(COINS);
                _currentShop = JsonConvert.DeserializeObject<Dictionary<string, int>>(serializedShop);
            }
            
            OnAuthEnd?.Invoke(new AuthorizationResponse(playerName, false));
        }
    }
    
    public async Task<int> GetCoins(string playerName)
    {
        if (_haveInternetConnection)
        {
            var request = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{playerName}/");
            var rawResponse = await SendRequest(request);
            var response = JsonConvert.DeserializeObject<ObjectResponse>(rawResponse.downloadHandler.text);
            await SendLog($"Игрок {playerName} получает текущие ресурсы.", playerName, response.resources);
            return response.resources[COINS];
        }
        else
            return _currentCoins;
    }
    
    public async void ChangeCoins(string playerName, int newCoinsCount)
    {
        if (_haveInternetConnection)
        {
            var res = new Dictionary<string, int> { { COINS, newCoinsCount } };
            var updatedRes = new UpdatePlayerResourceRequest { resources = res };
            var request = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{playerName}/", RequestType.PUT, updatedRes);
            await SendRequest(request);
            await SendLog($"Ресурсы игрока: {playerName}, были изменены. ", playerName, res);
        }
        else
            _currentCoins = newCoinsCount;
    }

    public async Task<Dictionary<string, int>> GetShop(string playerName)
    {
        if (_haveInternetConnection)
        {
            var request = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{playerName}/shops/");
            var rawResponse = await SendRequest(request);
            var response = JsonConvert.DeserializeObject<ObjectResponse[]>(rawResponse.downloadHandler.text);
            var list = new Dictionary<string, Dictionary<string, int>>();

            foreach (var objectResponse in response)
            {
                var name = objectResponse.name;
                var res = objectResponse.resources;
                list.Add(name, res);
            }

            await SendShopLog($"Игрок {playerName} получает текущий магазин. ", playerName, list[SHOP_NAME]);

            return list[SHOP_NAME];
        }
        else 
            return _currentShop;
    }

    public async void ChangeShop(string playerName, Dictionary<string, int> shop)
    {
        if (_haveInternetConnection)
        {
            var request = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{playerName}/shops/{SHOP_NAME}/", RequestType.PUT, shop);
            await SendRequest(request);
            await SendShopLog($"Магазин игрока {playerName} был изменен: {shop}", playerName, shop);
        }
        else 
            _currentShop = shop;
    }
    
    private async Task RegisterPlayer(string playerName)
    {
        var res = new Dictionary<string, int> { { COINS, 0 } };
        
        var registerPlayerObject = new RegisterPlayerRequest { name = playerName, resources =  res };
        var registerRequest = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/", RequestType.POST, registerPlayerObject);
        await SendRequest(registerRequest);
        await SendLog($"Игрок, {playerName}, зарегистирован. ", playerName, res);

        var shop = new Dictionary<string, int>
        {
            { Constants.FACE_SCANNER, 1 },
            { Constants.ANTI_CRIME_SYSTEM, 1 },
            { Constants.PIGGY_BANK, 1 },
            { Constants.HAPPY_HONEY, 1 },
            { Constants.HONORARY_CORPORATION, 1 }
        };
        
        var createShopObject = new CreateShopRequest { name = SHOP_NAME, resources = shop };
        var createShopRequest = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{playerName}/shops/", RequestType.POST, createShopObject);
        await SendRequest(createShopRequest);
        await SendShopLog($"Магазин игрока: {playerName} создан.", playerName, shop);
    }
    
    private async Task SendLog(string comment, string playerName, Dictionary<string, int> res)
    {
        var logObject = new RegisterPlayerLogRequest { comment = comment, player_name = playerName, resources_changed = res };
        var logRequest = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/", RequestType.POST, logObject);
        await SendRequest(logRequest);
    }

    private async Task SendShopLog(string comment, string playerName, Dictionary<string, int> shop)
    {
        var shopLogObject = new ShopLogRequest() { comment = comment, player_name = playerName, shop_name = SHOP_NAME, resources_changed = shop };
        var logRequest = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/logs/\n", RequestType.POST, shopLogObject);
        await SendRequest(logRequest);
    }
    
    private UnityWebRequest CreateRequest(string url, RequestType requestType = RequestType.GET, object requestBody = null)
    {
        var request = new UnityWebRequest(url, requestType.ToString());

        if (requestBody != null)
        {
            var jsonBody = JsonConvert.SerializeObject(requestBody);
            var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);

            Debug.Log($"Request Body: {jsonBody}");
        }
        
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        return request;
    }

    private async Task<UnityWebRequest> SendRequest(UnityWebRequest request)
    {
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log("Запрос успешно выполнен. Текст от сервера: " + request.downloadHandler.text);
        else
            Debug.Log("Ошибка зпроса. " + request.downloadHandler.error + " Текст от сервера: " + request.downloadHandler.text);

        return request;
    }
}

public enum RequestType
{
    GET,
    POST,
    PUT
}