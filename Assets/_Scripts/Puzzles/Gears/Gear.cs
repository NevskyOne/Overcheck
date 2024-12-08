using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gear : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private float _zPos;
    [SerializeField] private int _size;
    
    private Canvas _canvas;
    private Node _currentNode;
    private Vector2 _startPos;

    private GearPuzzlePreset _preset => transform.parent.parent.parent.parent.GetComponent<GearPuzzlePreset>();
    private GearPuzzle _gears => _preset.transform.parent.GetComponent<GearPuzzle>();

    public Node CurrentNode => _currentNode;
    public int Size => _size;

    public void Init(Canvas canvas)
    {
        _canvas = canvas;
        _startPos = transform.localPosition;
    }

    public void ResetPos()
    {
        transform.localPosition = _startPos;
        if (_currentNode != null)
        {
            _currentNode.Unsign();
            _currentNode = null;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = GetMouseWorldPosition();
        if (_currentNode != null)
        {
            _currentNode.Unsign();
            _currentNode = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var hoveredObject = GetHoveredUIObject(eventData);

        if (hoveredObject == null)
        {
            transform.localPosition = _startPos;
            return;
        }
        
        if (hoveredObject.TryGetComponent(out Node node))
        {
            if (node.IsAsigned)
            {
                transform.localPosition = _startPos;
                return;
            }
            
            Debug.Log($"Над объектом: {hoveredObject.name}");
            transform.position = node.transform.position;
            _currentNode = node;
            _currentNode.Asign();
            
            if (_preset.IsCorrect())
            {
                _gears.CheckPuzzle();
            }
        }
        else
            transform.localPosition = _startPos;
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = _zPos;
        
        var worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        var localPos =  transform.parent.InverseTransformPoint(worldPos);
        return new Vector3(localPos.x, localPos.y, 0);
    }
    
    private GameObject GetHoveredUIObject(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();

        var raycaster = _canvas.GetComponent<GraphicRaycaster>();
        raycaster.Raycast(eventData, results);

        if (results.Count > 1)
        {
            if (results[1].gameObject != null)
                return results[1].gameObject;
        }

        return null;
    }
}