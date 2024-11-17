using System;
using UnityEngine;

public struct RandomParamSt
{
    public static readonly string[] Names = {"Медведь", "Мишка"};
    public static readonly Sprite[] Photos = {Resources.Load<Sprite>("Sprites/BearPhotos/pig")};
    public static readonly Sprite[] Stamps =
    {
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig")
    };
}
