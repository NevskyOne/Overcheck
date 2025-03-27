using System;
using System.Collections.Generic;

[Serializable]
public class ModInfo
{
    public string Name;
    public string Description;
    public string Version;
    public string FolderPath;
    public string Id;
    public int Priority;
    public List<string> Dependencies = new();
    public List<string> DllsPath = new();
}