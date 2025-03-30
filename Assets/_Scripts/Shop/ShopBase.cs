using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ShopBase : MonoBehaviour
{
     [SerializeField] private List<Food> _shopFood = new();
     [SerializeField] private Transform _spawnPos;
     
     [Inject] private DiContainer _container;
     
     public void InitializePlayerShop() {}

     public async void BuyFood(int foodToBuy)
     {
         var playerName = AuthBootstrap.Instance.PlayerName;
         var coins = await APIManager.Instance.GetCoins(playerName);
         var food = _shopFood[foodToBuy];

         var price = food.Quality switch
         {
             FoodQuality.Bad => 2,
             FoodQuality.Normal => 5,
             FoodQuality.Exquisite => 15,
         };
         
         if (coins >= price)
         {
             _container.InstantiatePrefab(food, _spawnPos.position, Quaternion.identity, null);
             Coins.ChangeCoins(-price);
         }
     }
    
}