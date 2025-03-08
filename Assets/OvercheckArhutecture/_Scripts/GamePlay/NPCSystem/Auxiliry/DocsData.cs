using System;

[Serializable]
public class DocsData
{
    public bool Fake;
    public DocData[] Docs;
}

public abstract class DocData
{
    public string Name;
    public int Photo;
    public int Document;
}

public class PMSData : DocData
{
    public bool Male;
}

public class IICData : DocData
{
    public int HealthClass;
    public int ID;
    public int Stamp;
}

public class PPDData : DocData
{
    public int StartPlanet;
    public int EndPlanet;
    public int StartDate;
    public int StartMonth;
    public int EndDate;
    public int EndMonth;
}