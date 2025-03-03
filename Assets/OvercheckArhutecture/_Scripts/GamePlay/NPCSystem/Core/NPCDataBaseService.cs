public class NPCDataBaseService : IDataBaseService
{
    private NPCDataBase _dataBase;

    public NPCDataBaseService()
    {
        _dataBase = new NPCDataBase();
    }
    
    public void AddToDB(object obj)
    {
        var npc = (NPCData)obj;
        _dataBase.NPCDatas.Add(npc);
    }

    public object GetDB()
    {
        return _dataBase.NPCDatas;
    }
}