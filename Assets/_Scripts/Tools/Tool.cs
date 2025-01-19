
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    [SerializeField] private ToolName _toolName;
    public ToolName ToolName => _toolName;
}