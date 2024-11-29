using UnityEngine;

public abstract class ToolBase : MonoBehaviour
{
    [SerializeField] private ToolName _toolName;
    [SerializeField] private int _price;

    public string ToolName => _toolName.ToString();
    public int Price => _price;
}