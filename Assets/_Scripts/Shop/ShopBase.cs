using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ShopBase : MonoBehaviour
{
     [SerializeField] private List<Food> _shopFood = new();
     [SerializeField] private Transform _spawnPos;
     
     [Inject] private DiContainer _container;
     private static int _badPrice = 2;
     private static int _normalPrice = 5;
     private static int _exquisitePrice = 10;
     
     public void InitializePlayerShop() {}

     public async void BuyFood(int foodToBuy)
     {
         var playerName = AuthBootstrap.Instance.PlayerName;
         var coins = await APIManager.Instance.GetCoins(playerName);
         var food = _shopFood[foodToBuy];

         var price = food.Quality switch
         {
             FoodQuality.Bad => _badPrice,
             FoodQuality.Normal => _normalPrice,
             FoodQuality.Exquisite => _exquisitePrice,
         };
         
         if (coins >= price)
         {
             _container.InstantiatePrefab(food, _spawnPos.position, Quaternion.identity, null);
             PlayerCoins.ChangeCoins(-price);
         }
     }

     public static void ChangePrices(int badPrice, int normalPrice, int exquisitePrice)
     {
         _badPrice = badPrice;
         _normalPrice = normalPrice;
         _exquisitePrice = exquisitePrice;
     }
}