using UnityEngine;

public class Food: Movable, IUsable
{
    [SerializeField] private FoodQuality _quality;
    public FoodQuality Quality => _quality;

    public virtual void Use()
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
