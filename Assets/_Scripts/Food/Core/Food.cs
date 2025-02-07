using UnityEngine;

public class Food: Movable, IUsable
{
    [SerializeField] private FoodQuality _quality;
    //private uint _freshness = 2; надо ли нам?

    public void Use()
    {
        Saturation.ChangeSaturation((uint)_quality);
        Destroy(gameObject);
    }
}

public enum FoodQuality
{
    Bad = 10,
    Normal = 30,
    Exquisite = 80
}
