using System;

public class EventHasBeenInvoked
{
    public readonly Action Action;

    public EventHasBeenInvoked(Action action)
    {
        Action = action;
    }
}