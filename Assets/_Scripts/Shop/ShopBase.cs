using System.Collections.Generic;
using UnityEngine;

public class ShopBase : MonoBehaviour
{
     [SerializeField] private bool _needToActivate;
     [SerializeField] private List<Food> _shopFood = new();
     [SerializeField] private Transform _spawnPos;
     
     public async void InitializePlayerShop()
     {
         var shop = await APIManager.Instance.GetShop(AuthBootstrap.Instance.PlayerName);

         if (shop != null)
         {
             foreach (var tool in shop)
             {
                 if (tool.Value == 0)
                 {
                     var toolToAdd = _shopFood[tool.Value];
                     Instantiate(toolToAdd, _spawnPos.position, Quaternion.identity);
                     var key = tool.Key;
                     Debug.Log(key);
                 }
             }
         }
         APIManager.Instance.ChangeCoins(AuthBootstrap.Instance.PlayerName, 1000);
     }

     public async void BuyFood(string foodToBuy)
     {
         var playerName = AuthBootstrap.Instance.PlayerName;
         var coins = await APIManager.Instance.GetCoins(playerName);
         var food = GetTool(foodToBuy);

         var price = food.Quality switch
         {
             FoodQuality.Bad => 2,
             FoodQuality.Normal => 5,
             FoodQuality.Exquisite => 15,
         };
         var shop = await APIManager.Instance.GetShop(playerName);
         
         
         if (coins >= price)
         {
             shop[foodToBuy] = 0;
             APIManager.Instance.ChangeShop(playerName, shop);
             Instantiate(food, _spawnPos.position, Quaternion.identity);
             PlayerCoins.ChangeCoins(price, false);
         }
     }

     private Food GetTool(string foodName)
     {
         foreach (var food in _shopFood)
         {
             if (food.Title == foodName)
                 return food;
         }

         return null;
     }
}