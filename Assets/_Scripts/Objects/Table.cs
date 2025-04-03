
using UnityEngine;

public class Table : CamMove
{
    [SerializeField] private GameObject _ui;
    protected override void ChangePlayerState() => Player.State = PlayerState.Checking;

    public override void Interact()
    {
        base.Interact();
        _ui.SetActive(true);
    }
}
