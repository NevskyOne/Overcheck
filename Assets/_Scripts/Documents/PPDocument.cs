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
    
    public override void Setup(DocData docData)
    {
        base.Setup(docData);
        
        PPDData ppdData = docData as PPDData;

        _startPlanetText.text = ppdData.StartPlanet.ToString();
        _endPlanetText.text = ppdData.EndPlanet.ToString();
        _startDateText.text = ppdData.StartDate.ToString();
        _startMonthText.text = ppdData.StartMonth.ToString();
        _endDateText.text = ppdData.EndDate.ToString();
        _endMonthText.text = ppdData.EndMonth.ToString();

    }
    
    // public override void Randomize(int maxRandomCount)
    // {
    //     base.Randomize(maxRandomCount);
    //     try
    //     {
    //         var randomCount = _rnd.Next(0, maxRandomCount);
    //         for (UInt16 i = 0; i < randomCount; i++)
    //         {
    //             var randomParam = _rnd.Next(0, _paramCount);
    //             switch (randomParam)
    //             {
    //                 case 0:
    //                     _name = (RandomParamStruct.Names.Except(new List<string>{_name})).ToList()
    //                         [_rnd.Next(0,RandomParamStruct.Names.Count)];
    //                     break;
    //                 case 1:
    //                     _photo = (RandomParamStruct.Photos.Except(new List<Sprite>{_photo})).ToList()
    //                         [_rnd.Next(0,RandomParamStruct.Photos.Count)];
    //                     OnFaceChanging();
    //                     break;
    //                 case 2:
    //                     _startDate = _rnd.Next(1,28);
    //                     _startMonth = _rnd.Next(_curentMounth+1, 13);
    //                     break;
    //                 case 3:
    //                     _endDate = _rnd.Next(1,28);
    //                     _endMonth = _rnd.Next(1, _curentMounth);
    //                     break;
    //                 case 4:
    //                     var newPlanet = _startPlanet;
    //                     while (_startPlanet == newPlanet)
    //                         newPlanet = (Planet) _rnd.Next(0,5);
    //                     _startPlanet = newPlanet;
    //                     break;
    //                 case 5:
    //                     _endPlanet  = (Planet) _rnd.Next(1,5);
    //                     break;
    //             }
    //         }
    //         _nameText.text = _name;
    //         _photoImage.sprite = _photo;
    //         _startPlanetText.text = _startPlanet.ToString();
    //         _endPlanetText.text = _endPlanet.ToString();
    //         _startDateText.text = _startDate.ToString();
    //         _startMonthText.text = _startMonth.ToString();
    //         _endDateText.text = _endDate.ToString();
    //         _endMonthText.text = _endMonth.ToString();
    //     }
    //     catch(Exception e)
    //     {
    //         print(e.ToString());
    //     }
    // }
}