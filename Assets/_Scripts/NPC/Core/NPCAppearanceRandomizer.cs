using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCAppearanceRandomizer
{
    private readonly Mesh[] _models;
    private readonly Material[] _materials;
    private readonly Material[] _accessMaterials;

    public NPCAppearanceRandomizer(AppearRandomStruct structure)
    {
        _models = structure.Models;
        _materials = structure.Materials;
        _accessMaterials = structure.AccessMaterials;
    }

    public NPCAppearanceData Randomize(bool male)
    {
        var randomModelInd = Random.Range(0, _models.Length);

        var randomMatInd = Random.Range(0, _materials.Length);
        
        var randomAccess = Random.Range(0, _accessMaterials.Length);
        
        var photo = (randomModelInd) * 10 + (int)Mathf.Ceil((randomMatInd + 1) / 3f) - 1;
        
        List<int> accessories = new(){Random.Range(0, 3)}; //Hat
        int randomCount = Random.Range(0, 5);
        for (var i = 0; i < randomCount; i++)
            accessories.Add(Random.Range(3, 7));
        
        return new NPCAppearanceData
        {
            Model = randomModelInd,
            ModelMaterial = randomMatInd,
            AccessMaterial = randomAccess,
            Photo = photo,
            Accessories = accessories.ToArray()
        };
    }
}

[Serializable]
public struct AppearRandomStruct
{
    public Mesh[] Models;
    public Material[] Materials;
    public Material[] AccessMaterials;
    public NPCBase BasePrefab;
}