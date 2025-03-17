public class CreateNPCRequestEvent
{
    public readonly int Count;
    public readonly bool IsNewGame;

    public CreateNPCRequestEvent(int count, bool isNewGame)
    {
        Count = count;
        IsNewGame = isNewGame;
    }
}