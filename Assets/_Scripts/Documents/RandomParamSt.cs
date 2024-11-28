using System;
using System.Collections.Generic;
using UnityEngine;

public struct RandomParamSt
{
    public static readonly List<string> Names = new(){"Медведь", "Мишка", "Фредди Фазбер", "Фредди Крюгер"};
    public static readonly List<Sprite> Photos = new(){Resources.Load<Sprite>("Sprites/BearPhotos/pig")};
    public static readonly List<Sprite> Stamps = new()
    {
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig")
    };
}
