using System;
using UnityEngine;

public class IICDocument : Document
{
    private int _healthClass;
    private int _ID;
    private Sprite _stamp;
    
    public IICDocument(string name, Sprite photo, int planet) : base(name, photo)
    {
        _healthClass = _rnd.Next(3,6);
        _ID = _rnd.Next(10000,99999);
        _stamp = RandomParamSt.Stamps[planet];
        _paramCount = 4;
    }
    
    public override void Randomize(int maxRandomCount)
    {
        var randomCount = _rnd.Next(1, maxRandomCount);
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
                case 2:
                    _healthClass = _rnd.Next(1,3);
                    break;
                case 3:
                    _ID = _rnd.Next(10000,99999);
                    break;
                case 4:
                    var newStamp = _stamp;
                    while (_stamp == newStamp)
                        newStamp = RandomParamSt.Stamps[_rnd.Next(0,RandomParamSt.Stamps.Length)];
                    break;
            }
        }
    }
}
