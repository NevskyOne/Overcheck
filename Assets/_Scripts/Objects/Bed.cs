public class Bed : CamMove
{
    public new int CursorInd { get; set; } = 3;
    protected override void ChangePlayerState() => Player.State = PlayerState.Block;

    public override void Interact()
    {
        base.Interact();
        _player.Model.SetActive(false);
    }
}
