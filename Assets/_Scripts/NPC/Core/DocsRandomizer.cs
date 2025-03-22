using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Random = UnityEngine.Random;


public class DocsRandomizer
{
    private List<string> _maleNames = new (RandomParamStruct.MaleNames);
    private List<string> _femaleNames = new (RandomParamStruct.FemaleNames);
    private List<string> _criminalMale = new(), _criminalFemale = new();
    private int _criminalCount, _criminalChance, _fakeChance;

    public DocsRandomizer(int fakeChance, int criminalChance, int criminalCount)
    {
        _fakeChance = fakeChance;
        _criminalChance = criminalChance;
        _criminalCount = criminalCount;
    }

    public List<string> CreateCriminals()
    {
        for (int i = 0; i < _criminalCount; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                var name = _maleNames[Random.Range(0, _maleNames.Count)];
                _criminalMale.Add(name);
                _maleNames.Remove(name);
            }
            else
            {
                var name = _femaleNames[Random.Range(0, _femaleNames.Count)];
                _criminalFemale.Add(name);
                _femaleNames.Remove(name);
            }
        }

        var newList = new List<string>(_criminalMale);
        newList.AddRange(_criminalFemale);
        return newList;
    }
    
    private string ChooseName(bool male, bool criminal)
    {
        string name;
        if (criminal)
        {
            if (male)
            {
                name = _criminalMale[Random.Range(0, _criminalMale.Count)];
                _criminalMale.Remove(name);
            }
            else
            {
                name = _criminalFemale[Random.Range(0, _criminalFemale.Count)];
                _criminalFemale.Remove(name);
            }
        }
        else
        {
            if (male)
            {
                name = _maleNames[Random.Range(0, _maleNames.Count)];
                _maleNames.Remove(name);
            }
            else
            {
                name = _femaleNames[Random.Range(0, _femaleNames.Count)];
                _femaleNames.Remove(name);
            }
        }

        return name;
    }
    
    public DocsData Randomize(bool male, int photo, DocumentDataBase _dataBase)
    {
        var criminal = (male? _criminalMale.Count > 0: _criminalFemale.Count > 0) && 
                       (Random.Range(1,101) < _criminalChance);
        var fake = criminal || Random.Range(1,101) < _fakeChance;
        
        PMSData PMS = new();
        IICData IIC = new();
        PPData PP = new();
        
        var name = ChooseName(male,criminal);
        var id = Random.Range(100000, 999999);
        var healthClass = Random.Range(3, 6);
        var planet = Random.Range(1, 5);
        var endPlanet = 0;
        var startDate = Random.Range(1, 29);
        var startMonth = Random.Range(1, 6);
        var endDate = Random.Range(1, 29);
        var endMonth = Random.Range(7, 13);
        
        _dataBase.AddToDB(new BearData
        {
            Name = name,
            ID = (uint)id,
            Photo = photo
        });
        
        
        
        if (fake)
        {
            var newPhotoes = RandomParamStruct.Photos.Except(RandomParamStruct.Photos[photo]).ToList();
            switch (Random.Range(0,2)) //first documnet
            {
                case 0:
                    photo = Random.Range(0, newPhotoes.Count);
                    break;
                case 1:
                    male = !male;
                    break;
            }
            
            PMS = new PMSData
            {
                Name = name,
                Document = 0,
                Photo = photo,
                Male = male
            };
            
            switch (Random.Range(0, 5)) //second documnet
            {
                case 0:
                    name = ChooseName(male, false);
                    break;
                case 1:
                    photo = Random.Range(0, newPhotoes.Count);
                    break;
                case 2:
                    healthClass = Random.Range(1, 3);
                    break;
                case 3:
                    var newId = id;
                    while (newId == id)
                    {
                        newId = Random.Range(100000, 999999);
                    }
                    id = newId;
                    break;
                case 4:
                    planet = 0;
                    break;
            }
            
            IIC = new IICData
            {
                Name = name,
                Document = 1,
                Photo = photo,
                HealthClass = healthClass,
                ID = id,
                Stamp = planet
            };
            
            switch (Random.Range(0, 6)) //third documnet
            {
                case 0:
                    name = ChooseName(male, false);
                    break;
                case 1:
                    photo = Random.Range(0, newPhotoes.Count);
                    break;
                case 2:
                    startMonth = Random.Range(7, 13);
                    break;
                case 3:
                    endMonth = Random.Range(1, 6);
                    break;
                case 4:
                    planet = 0;
                    break;
                case 5:
                    endPlanet = Random.Range(1,5);
                    break;
            }
            
            PP = new PPData
            {
                Name = name,
                Document = 2,
                Photo = photo,
                StartDate = startDate,
                StartMonth = startMonth,
                EndDate = endDate,
                EndMonth = endMonth,
                StartPlanet = planet,
                EndPlanet = endPlanet
            };
        }
        else
        {
            PMS = new PMSData
            {
                Name = name,
                Document = 0,
                Photo = photo,
                Male = male
            };
            IIC = new IICData
            {
                Name = name,
                Document = 1,
                Photo = photo,
                HealthClass = healthClass,
                ID = id,
                Stamp = planet
            };
            PP = new PPData
            {
                Name = name,
                Document = 2,
                Photo = photo,
                StartDate = startDate,
                StartMonth = startMonth,
                EndDate = endDate,
                EndMonth = endMonth,
                StartPlanet = planet,
                EndPlanet = endPlanet
            };
        }

        return new DocsData
        {
            Fake = fake,
            Criminal = criminal,
            Docs = new DocData[]{PMS, IIC, PP}
        };
    }
}