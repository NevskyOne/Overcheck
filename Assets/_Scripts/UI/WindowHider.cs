
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindowHider : MonoBehaviour
{
    [SerializeField] private List<GameObject> _windows;

    public void HideWindows(GameObject obj)
    {
        foreach (var window in _windows.Except(new []{obj})) window.SetActive(false);
    }
}
