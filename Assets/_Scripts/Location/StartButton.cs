using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Material _material;
    private bool enable;

    public bool Enabled
    {
        get
        {
            return enable;
        }
        set
        {
            _material.color = value ? Color.green : Color.grey;
            enable = value;
        }
    }

    private void Start()
    {
        _material.color = Color.grey;
    }
}
