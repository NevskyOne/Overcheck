using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IICDocument : Document
{
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _IDText;
    [SerializeField] private Image _stampImage;
    
    private int _healthClass;
    private int _ID;
    private Sprite _stamp;
    
    public override void Initialize(string name, Sprite photo, int planet)
    {
        base.Initialize(name, photo, planet);
        
        _healthClass = _rnd.Next(3,6);
        _ID = _rnd.Next(10000,99999);
        _stamp = RandomParamSt.Stamps[planet];
        _paramCount = 4;

        _healthText.text = _healthClass.ToString();
        _IDText.text = _ID.ToString();
        _stampImage.sprite = _stamp;
    }
    
    public override void Randomize(int maxRandomCount)
    {
        base.Randomize(maxRandomCount);
        
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
