public class Bed : CamMove
{
    protected override void ChangePlayerState() => Player.State = PlayerState.Block;

    public override void Interact()
    {
        base.Interact();
        _player.Model.SetActive(false);
    }
}
