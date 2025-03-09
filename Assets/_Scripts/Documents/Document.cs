using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public abstract class Document : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] protected TMP_Text _nameText;
    [SerializeField] protected Image _photoImage;

    private float _initHeight;
    private Rigidbody _rb => GetComponent<Rigidbody>();
    
    public virtual void Setup(DocData docData)
    {
        _nameText.text = docData.Name;
        _photoImage.sprite = RandomParamStruct.Photos[docData.Photo];
    }

    public void Move(Vector3 position)
    {
        if(!_rb.isKinematic) return;
        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }

    public void SetInitialHeight(float height) => _initHeight = height;
    
    public void Interact()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        transform.position =
            new Vector3(transform.position.x, _initHeight + 0.2f, transform.position.z);
    }

    public void Uninteract()
    {
        _rb.useGravity = true;
        _rb.isKinematic = false;
    }
}


