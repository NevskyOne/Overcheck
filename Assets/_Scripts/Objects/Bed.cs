using System;

public class Bed : CamMove
{
    protected override void ChangePlayerState() => Player.State = PlayerState.Block;

    private bool _canSleep;
    

    private void Start()
    {
        CursorInd = 3;
        _eventBus.Subscribe((AllDayNPCsEndedEvent _) => { _canSleep = true;});
        _eventBus.Subscribe((NewDayStartedEvent _) => { _canSleep = false;});
    }

    public override void Interact()
    {
        if (!_canSleep) return;
        _player.Model.SetActive(false);
        base.Interact();
        _mainUI.Sleep();
    }

    public override void Uninteract()
    {
        base.Uninteract();
        _player.Model.SetActive(true);
    }
}
