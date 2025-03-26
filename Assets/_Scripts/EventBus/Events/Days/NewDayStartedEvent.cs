public class NewDayStartedEvent
{
    public readonly DayData DayData;

    public NewDayStartedEvent(DayData dayData)
    {
        DayData = dayData;
    }
}