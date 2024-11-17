using System;
using UnityEngine;
using Random = System.Random;

public abstract class Document
{
    protected string _name;
    protected Sprite _photo;
    protected bool _origin = true;
    protected int _paramCount = 2;
    protected Random _rnd = new Random();

    public Document(string name, Sprite photo)
    {
        _name = RandomParamSt.Names[_rnd.Next(0,RandomParamSt.Names.Length)];
        _photo = photo;
    }
    
    public abstract void Randomize(int maxRandomCount);
}
