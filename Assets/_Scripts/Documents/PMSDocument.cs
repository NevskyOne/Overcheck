using System;
using UnityEngine;

public class PMSDocument : Document
{
    public PMSDocument(string name, Sprite photo) : base(name, photo)
    { }
    
    public override void Randomize(int maxRandomCount)
    {
        var randomCount = _rnd.Next(0, maxRandomCount);
        for (UInt16 i = 0; i < randomCount; i++)
        {
            var randomParam = _rnd.Next(0, _paramCount);
            switch (randomParam)
            {
                case 0:
                    var newName = _name;
                    while (_name == newName)
                        newName = RandomParamSt.Names[_rnd.Next(0,RandomParamSt.Names.Length)];
                    break;
                case 1:
                    var newPhoto = _photo;
                    while (_photo == newPhoto)
                        newPhoto = RandomParamSt.Photos[_rnd.Next(0,RandomParamSt.Photos.Length)];
                    break;
            }
        }
    }
}