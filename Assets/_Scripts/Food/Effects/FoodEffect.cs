using System;
using UnityEngine;

[Serializable]
public abstract class FoodEffect
{
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
}
