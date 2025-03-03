
public class Table : CamMove
{
    protected override void ChangePlayerState() => Player.State = PlayerState.Checking;
}
