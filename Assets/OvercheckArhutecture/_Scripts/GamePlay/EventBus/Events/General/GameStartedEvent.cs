public class GameStartedEvent
{
    public readonly bool IsNewGame;

    public GameStartedEvent(bool isNewGame)
    {
        IsNewGame = isNewGame;
    }
}