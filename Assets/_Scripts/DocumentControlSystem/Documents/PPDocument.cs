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
    
    public override void Setup(DocData docData)
    {
        base.Setup(docData);
        
        PPData ppData = docData as PPData;

        _startPlanetText.text = ((Planet)Enum.GetValues(typeof(Planet)).GetValue(ppData.StartPlanet)).ToString();
        _endPlanetText.text = ((Planet)Enum.GetValues(typeof(Planet)).GetValue(ppData.EndPlanet)).ToString();
        _startDateText.text = ppData.StartDate.ToString();
        _startMonthText.text = ppData.StartMonth.ToString();
        _endDateText.text = ppData.EndDate.ToString();
        _endMonthText.text = ppData.EndMonth.ToString();

    }
}