public interface ISaver
{
    void Save<T>(T obj, string path);
    T Load<T>(string path);
}