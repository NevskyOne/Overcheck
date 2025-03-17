using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private int _size;

    public int Size => _size;
    public bool IsAsigned { get; private set; }
    
    public void Asign() => IsAsigned = true;
    
    public void Unsign() => IsAsigned = false;
}