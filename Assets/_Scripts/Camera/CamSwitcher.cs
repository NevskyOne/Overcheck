using System;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitcher : MonoBehaviour
{
    [SerializeField] private List<CamMove> _camList;
    private int _currentIndex;

    private void Start()
    {
        for (var i = 0; i < _camList.Count; i++)
        {
            _camList[0].OnInteract += () =>
            {
                _currentIndex = i;
                Player.State = PlayerState.CamSwitcher;
                Player.Interactions.Switcher = this;
            };
        }
    }
    
    public void SwitchCamMove(Vector2 direction)
    {
        int newIndex = GetIndexByDirection(direction);
        if (newIndex != -1 && newIndex != _currentIndex && newIndex < _camList.Count)
        {
            _camList[newIndex]?.Interact();
        }
    }

    private int GetIndexByDirection(Vector2 direction)
    {
        if (direction == Vector2.left) return 0;
        if (direction == Vector2.up) return 1;
        if (direction == Vector2.right) return 2;
        if (direction == Vector2.down) return 3;
        return -1;
    }
}
