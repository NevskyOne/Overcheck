public class DayEndedEvent
{
    public readonly DayData DayData;

    public DayEndedEvent(DayData dayData)
    {
        DayData = dayData;
    }
}