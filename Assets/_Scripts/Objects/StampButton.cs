using System;
using UnityEngine;

public class StampButton : MonoBehaviour, IInteractable
{
    [SerializeField] private CheckState _checkState;
    [SerializeField] private Material _material;
    [SerializeField] private Color _activeColor;
    [SerializeField] private StampButton _anotherButton;

    private Color _defaultColor;
    public int CursorInd { get; set; } = 2;

    private void Start()
    {
        _defaultColor = _material.color;
    }

    public void Interact()
    {
        if (Player.CheckingState == _checkState)
        {
            Uninteract();
            return;
        }
        _anotherButton.Uninteract();

        ;
        _material.color = _activeColor;

        Player.CheckingState = _checkState;
    }

    public void Uninteract()
    {
        Player.CheckingState = CheckState.None;
        _material.color = _defaultColor;
    }

    private void OnDisable()
    {
        _material.color = _defaultColor;
    }
}
