using System;
using System.Linq;
using ModestTree;
using Random = UnityEngine.Random;
// ReSharper disable All

public class DocsRandomizer
{
    public DocsData Randomize(bool male, int photo)
    {
        var fake = Random.Range(0,100) > 79;
        PMSData PMS = new();
        IICData IIC = new();
        PPDData PPD = new();
        
        var name = male? 
            RandomParamStruct.MaleNames[Random.Range(0,RandomParamStruct.MaleNames.Count)] :
            RandomParamStruct.FemaleNames[Random.Range(0,RandomParamStruct.FemaleNames.Count)];
        var id = Random.Range(100000, 999999);
        var healthClass = Random.Range(3, 6);
        var planet = Random.Range(1, 5);
        var startDate = Random.Range(1, 29);
        var startMonth = Random.Range(7, 12);
        var endDate = Random.Range(1, 29);
        var endMonth = Random.Range(startMonth, 13);
        
        
        if (fake)
        {
            var newNames = male? 
                RandomParamStruct.MaleNames.Except(name).ToList() :
                RandomParamStruct.FemaleNames.Except(name).ToList();
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
                    name = newNames[Random.Range(0, newNames.Count)];
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
            PPD = new PPDData
            {
                Name = name,
                Document = 2,
                Photo = photo,
                StartDate = startDate,
                StartMonth = startMonth,
                EndDate = endDate,
                EndMonth = endMonth,
                StartPlanet = planet,
                EndPlanet = 0
            };
        }

        return new DocsData
        {
            Fake = fake,
            Docs = new DocData[]{PMS, IIC, PPD}
        };
    }
}