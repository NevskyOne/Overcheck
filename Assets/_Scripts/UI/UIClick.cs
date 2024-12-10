
using UnityEngine.EventSystems;
using UnityEngine;

public class UIClick : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _click;
    [SerializeField] private float _zPos;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
        {
            _click.transform.localPosition = GetMouseWorldPosition();
            _click.SetActive(true);
        }
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = _zPos;
        
        var worldPos = _cam.ScreenToWorldPoint(mousePosition);
        var localPos =  transform.InverseTransformPoint(worldPos);
        return new Vector3(localPos.x, localPos.y, 0);
    }
    
}
