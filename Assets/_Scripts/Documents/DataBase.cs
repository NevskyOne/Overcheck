using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataBase : MonoBehaviour
{
    [SerializeField] private Transform _spawnPrefab;
    [SerializeField] private GameObject _bearPrefab;
    private List<BearData> _bears = new();

    public void AddBear(BearData bear) => _bears.Add(bear);

    public void ClearData()
    {
        RemoveChildren();
        _bears.Clear();
    }

    public void CheckForId(string id)
    {
        RemoveChildren();
        foreach (var bear in _bears.Where(bear => id != "" && bear.ID.ToString().StartsWith(id)))
        {
            SpawnBear(bear);
        }
    }

    private void RemoveChildren()
    {
        for (var i = 0; i < _spawnPrefab.childCount; i++)
        {
            Destroy(_spawnPrefab.GetChild(i).gameObject);
        }
    }
    
    private void SpawnBear(BearData bearData)
    {
        var newObj = Instantiate(_bearPrefab, _spawnPrefab).transform;
        newObj.GetChild(0).GetComponent<TMP_Text>().text = bearData.Name;
        newObj.GetChild(1).GetComponent<Image>().sprite = bearData.Photo;
        newObj.GetChild(2).GetComponent<TMP_Text>().text = bearData.ID.ToString();
    }
}

public struct BearData
{
    public string Name;
    public Sprite Photo;
    public uint ID;
}
