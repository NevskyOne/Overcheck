using System;
using System.Collections.Generic;
using System.Linq;
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
    private DataBase _dataBase => FindFirstObjectByType<DataBase>();
    
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
        
        _dataBase.AddBear(new BearData
        {
            Name = _name,
            Photo = _photo,
            ID = (uint)_ID
        });
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
                    _name = (RandomParamSt.Names.Except(new List<string>{_name})).ToList()
                        [_rnd.Next(0,RandomParamSt.Names.Count)];
                    break;
                case 1:
                    _photo = (RandomParamSt.Photos.Except(new List<Sprite>{_photo})).ToList()
                        [_rnd.Next(0,RandomParamSt.Photos.Count)];
                    OnFaceChanging();
                    break;
                case 2:
                    _healthClass = _rnd.Next(1,3);
                    break;
                case 3:
                    _ID = _rnd.Next(10000,99999);
                    break;
                case 4:
                    _stamp = (RandomParamSt.Stamps.Except(new List<Sprite>{_stamp})).ToList()
                        [_rnd.Next(0,RandomParamSt.Stamps.Count)];
                    break;
            }
        }
        _nameText.text = _name;
        _photoImage.sprite = _photo;
        _healthText.text = _healthClass.ToString();
        _IDText.text = _ID.ToString();
        _stampImage.sprite = _stamp;
    }
}
