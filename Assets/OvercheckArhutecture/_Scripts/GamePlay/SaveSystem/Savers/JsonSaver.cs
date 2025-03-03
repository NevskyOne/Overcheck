using System.IO;
using Newtonsoft.Json;

public class JsonSaver : ISaver
{
    public void Save<T>(T obj, string path)
    {
        var directory = Path.GetDirectoryName(path);
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public T Load<T>(string path)
    {
        if (!File.Exists(path)) return default;
        
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }
}