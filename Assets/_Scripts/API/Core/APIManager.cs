using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager
{
    private static APIManager _instance;
    public static APIManager Instance => _instance ??= new APIManager();

    private APIManager() { }
    
    private const string UUID = "a035437c-0e73-48c3-9d62-2da4ed3298eb";
    private const string COINS = "coins";
    private const string SHOP_NAME = "shop_base";

    public async Task<AuthorizationResponse> Authorization(string playerName)
    {
        var getPlayersListRequestRaw = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/");
        var getPlayersListRequest = await SendRequest(getPlayersListRequestRaw);
        await SendLog("Проверка сущетсвует ли игрок. ", playerName, new Dictionary<string, int>());
        
        var response = JsonConvert.DeserializeObject<PlayerResponse[]>(getPlayersListRequest.downloadHandler.text);
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
            return new AuthorizationResponse(playerName, false);
        }

        return new AuthorizationResponse(playerName, true);
    }
    
    public async Task<int> GetCoins(string playerName)
    {
        var request = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{playerName}/");
        var rawResponse = await SendRequest(request);
        var response = JsonConvert.DeserializeObject<PlayerResponse>(rawResponse.downloadHandler.text);
        return response.resources[COINS];
    }
    
    public async void ChangeCoins(string playerName, int newCoinsCount)
    {
        var res = new Dictionary<string, int> { { COINS, newCoinsCount } };
        var updatedRes = new UpdatePlayerResourceRequest { resources = res };
        var request = CreateRequest($"https://2025.nti-gamedev.ru/api/games/{UUID}/players/{playerName}/", RequestType.PUT, updatedRes);
        await SendRequest(request);
        await SendLog($"Ресурсы игрока: {playerName}, были изменены. ", playerName, res);
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