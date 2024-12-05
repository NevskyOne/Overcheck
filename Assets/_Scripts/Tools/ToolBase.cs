using UnityEngine;

public class ToolBase : MonoBehaviour
{
    [SerializeField] private ToolName _toolName;
    [SerializeField] private int _price;
    [SerializeField] private int _index;

    public string ToolName => _toolName.ToString();
    public int Price => _price;
    public int Index => _index;
}