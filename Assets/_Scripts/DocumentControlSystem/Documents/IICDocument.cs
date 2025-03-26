using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IICDocument : Document
{
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _IDText;
    [SerializeField] private Image _stampImage;
    
    public override void Setup(DocData docData)
    {
        base.Setup(docData);
        
        IICData iicData = docData as IICData;
        
        _healthText.text = iicData.HealthClass.ToString();
        _IDText.text = iicData.ID.ToString();
        _stampImage.sprite = RandomParamStruct.Stamps[iicData.Stamp];
        
        // _dataBase.AddBear(new BearData
        // {
        //     Name = docData.Name,
        //     Photo = RandomParamStruct.Photos[iicData.Photo],
        //     ID = (uint)iicData.ID
        // });
    }
}
