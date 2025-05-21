using UnityEngine;

public class Food: Movable, IUsable
{
    [SerializeField] private FoodQuality _quality;
    [SerializeField] private GameObject _particle;
    public FoodQuality Quality => _quality;

    public virtual void Use()
    {
        Saturation.ChangeSaturation((int)_quality);
        Instantiate(_particle, transform.position, Quaternion.Euler(90,0,0));
        Destroy(gameObject);
    }
}

public enum FoodQuality
{
    Bad = 10,
    Normal = 30,
    Exquisite = 80
}
