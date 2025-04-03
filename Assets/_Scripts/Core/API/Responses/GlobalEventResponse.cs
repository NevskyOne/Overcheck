using System;

[Serializable]
public class GlobalEventResponse
{
    public int id;
    public string name;
    public string text;
    public int once_in_hours;
    public int duration_in_minutes;
    public string start_date_time;
}