using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] private string _title;
    [SerializeField][TextArea] private string _description;

    public string Title => _title;
    public string Description => _description;
    
    private Rigidbody _rb => GetComponent<Rigidbody>();
    private Transform _rotator => GameObject.FindWithTag("Player").transform.GetChild(0).GetChild(0);
    
    public void Take()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        transform.SetParent(_rotator);
        transform.localPosition = new Vector3(0, -0.05f, 0.05f);
        _rotator.gameObject.SetActive(true);
    }
    
    public void Throw()
    {
        gameObject.SetActive(true);
        _rb.useGravity = true;
        _rb.isKinematic = false;
        
    }
}
