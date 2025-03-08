using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Movable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _title;
    [SerializeField][TextArea] private string _description;

    public string Title => _title;
    public string Description => _description;
    
    private Rigidbody _rb => GetComponent<Rigidbody>();
    private Collider _collider => GetComponent<Collider>();
    private DragRotate _rotator;
    private MainUI _mainUI;

    [Inject]
    private void Initialize(Player player, MainUI mainUI)
    {
        _rotator = player.Rotator;
        _mainUI = mainUI;
    }
    
    public void Interact()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        transform.SetParent(_rotator.transform);
        transform.localPosition = new Vector3(0, 0, 0.4f);
        _collider.enabled = false;
        _rotator.enabled = true;
        
        Player.State = PlayerState.Holding;
        _mainUI.HideCursor();
    }
    
    public void Uninteract()
    {
        gameObject.SetActive(true);
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _collider.enabled = true;
        
        Player.State = PlayerState.Movement;
        _mainUI.ShowCursor();
    }
}
