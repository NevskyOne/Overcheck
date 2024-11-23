using System;
using TMPro;
using UnityEngine;

public class PPDocument : Document
{
    [SerializeField] private TMP_Text _startPlanetText;
    [SerializeField] private TMP_Text _endPlanetText;
    [SerializeField] private TMP_Text _startDateText;
    [SerializeField] private TMP_Text _startMonthText;
    [SerializeField] private TMP_Text _endDateText;
    [SerializeField] private TMP_Text _endMonthText;
    
    private Planet _startPlanet;
    private Planet _endPlanet;
    private int _startDate;
    private int _startMonth;
    private int _endDate;
    private int _endMonth;

    private int _curentMounth;
    
    public override void Initialize(string name, Sprite photo, int planet)
    {
        base.Initialize(name, photo, planet);
        try
        {
            _curentMounth = FindFirstObjectByType<TimeLines>().Month;

            _startPlanet = (Planet)planet;
            _endPlanet = Planet.Planet1;
            _startDate = _rnd.Next(1, 28);
            _startMonth = _rnd.Next(1, _curentMounth);
            _endDate = _rnd.Next(1, 28);
            _endMonth = _rnd.Next(_curentMounth + 1, 13);
            _paramCount = 6;

            _startPlanetText.text = _startPlanet.ToString();
            _endPlanetText.text = _endPlanet.ToString();
            _startDateText.text = _startDate.ToString();
            _startMonthText.text = _startMonth.ToString();
            _endDateText.text = _endDate.ToString();
            _endMonthText.text = _endMonth.ToString();
        }
        catch(Exception e)
        {
            print(e.ToString());
        }
    }
    
    public override void Randomize(int maxRandomCount)
    {
        base.Randomize(maxRandomCount);
        try
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
                    case 2:
                        _startDate = _rnd.Next(1,28);
                        _startMonth = _rnd.Next(_curentMounth+1, 13);
                        break;
                    case 3:
                        _endDate = _rnd.Next(1,28);
                        _endMonth = _rnd.Next(1, _curentMounth);
                        break;
                    case 4:
                        var newPlanet = _startPlanet;
                        while (_startPlanet == newPlanet)
                            newPlanet = (Planet) _rnd.Next(1,5);
                        break;
                    case 5:
                        var newEndPlanet = _endPlanet;
                        while (newEndPlanet == Planet.Planet1)
                            newEndPlanet = (Planet) _rnd.Next(1,5);
                        break;
                }
            }
        }
        catch(Exception e)
        {
            print(e.ToString());
        }
    }
}