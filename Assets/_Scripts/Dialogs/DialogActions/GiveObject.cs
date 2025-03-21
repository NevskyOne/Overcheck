using System;
using UnityEngine;
using Zenject;

[Serializable]
public class GiveObject : IDialogAction
{
    [SerializeField] private GameObject _object;
    private DiContainer _container;

    public GiveObject(DiContainer container)
    {
        _container = container;
    }
    public void DoAction()
    {
        _container.InstantiatePrefab(_object, GameObject.FindGameObjectWithTag("ItemsHolder").transform);
    }
    public void AfterAction(){}
}
