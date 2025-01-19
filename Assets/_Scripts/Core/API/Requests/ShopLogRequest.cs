using System.Collections.Generic;

public class ShopLogRequest
{
    public string comment;
    public string player_name;
    public string shop_name;
    public Dictionary<string, int> resources_changed;
}