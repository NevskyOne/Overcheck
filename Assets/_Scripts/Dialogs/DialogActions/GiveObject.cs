using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class GiveObject : IDialogAction
{
    [SerializeField] private GameObject _object;
    public  void DoAction()
    {
        Object.Instantiate(_object, Object.FindFirstObjectByType<DragRotate>().transform);
    }
}
