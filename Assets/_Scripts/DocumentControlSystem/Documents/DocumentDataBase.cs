using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocumentDataBase : MonoBehaviour, IDataBaseService
{
    [SerializeField] private List<Transform> _spawnPrefabs;
    [SerializeField] private GameObject _bearPrefab;
    private static List<BearData> _bears = new();
    
    public void AddToDB(object obj)
    {
        _bears.Add((BearData)obj);
    }

    public object GetDB()
    {
        return _bears;
    }
    
    public void ClearData()
    {
        RemoveAllChildren();
        _bears.Clear();
    }

    public void CheckForId(string id)
    {
        RemoveChildren(0);
        foreach (var bear in _bears.Where(bear => id != "" && bear.ID.ToString().StartsWith(id)))
        {
            SpawnBear(bear, 0);
        }
    }

    private void RemoveAllChildren()
    {
        for (int i = 0; i < _spawnPrefabs.Count; i++)
        {
            RemoveChildren(i);
        }
    }
    
    private void RemoveChildren(int index)
    {
        for (var i = 0; i < _spawnPrefabs[index].childCount; i++)
        {
            Destroy(_spawnPrefabs[index].GetChild(i).gameObject);
        }
    }
    
    private void SpawnBear(BearData bearData, int index)
    {
        var newObj = Instantiate(_bearPrefab, _spawnPrefabs[index]).transform;
        newObj.GetChild(0).GetComponent<TMP_Text>().text = bearData.Name;
        newObj.GetChild(1).GetChild(0).GetComponent<Image>().sprite = RandomParamStruct.Photos[bearData.Photo];
        newObj.GetChild(2).GetComponent<TMP_Text>().text = bearData.ID.ToString();
    }

    public void InitDataBase()
    {
        RemoveAllChildren();
        foreach (var bear in _bears)
        {
            SpawnBear(bear,0);
            SpawnBear(bear,1);
        }
    }
}


public struct BearData
{
    public string Name;
    public int Photo;
    public uint ID;
}
