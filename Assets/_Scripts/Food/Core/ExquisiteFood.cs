using UnityEngine;

public class ExquisiteFood : Food
{
    [SerializeReference, SerializeReferenceButton] private FoodEffect _effect;

    public override void Use()
    {
        base.Use();
        _effect.ApplyEffect();
    }
}
