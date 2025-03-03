using UnityEngine;
using Zenject;

public class Movable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _title;
    [SerializeField][TextArea] private string _description;

    public string Title => _title;
    public string Description => _description;
    
    private Rigidbody _rb => GetComponent<Rigidbody>();
    private Transform _rotator;

    [Inject]
    private void Initialize(Player player)
    {
        _rotator = player.Rotator.transform;
    }
    
    public void Interact()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        transform.SetParent(_rotator);
        transform.localPosition = new Vector3(0, -0.05f, 0.05f);
        _rotator.gameObject.SetActive(true);
    }
    
    public void Uninteract()
    {
        gameObject.SetActive(true);
        _rb.useGravity = true;
        _rb.isKinematic = false;
        
    }
}
