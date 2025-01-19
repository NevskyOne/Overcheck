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

    private GearPuzzlePreset _preset;
    private GearPuzzle _gears;
    private Transform _scrollTransform;
    
    public Node CurrentNode => _currentNode;
    public int Size => _size;

    public void Init(Canvas canvas)
    {
        _canvas = canvas;
        _startPos = new Vector2(260,transform.localPosition.y);
        _scrollTransform = transform.parent;
        _gears = canvas.GetComponent<GearPuzzle>();
        _preset = _gears.transform.GetComponentInChildren<GearPuzzlePreset>();
    }

    public void ResetPos()
    {
        transform.SetParent(_scrollTransform);
        transform.localPosition = _startPos;
        if (_currentNode != null)
        {
            _currentNode.Unsign();
            _currentNode = null;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.SetParent(_gears.transform);
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
            transform.SetParent(_scrollTransform);
            transform.localPosition = _startPos;
            return;
        }
        
        if (hoveredObject.TryGetComponent(out Node node))
        {
            if (node.IsAsigned)
            {
                transform.SetParent(_scrollTransform);
                transform.localPosition = _startPos;
                return;
            }
            
            Debug.Log($"Над объектом: {hoveredObject.name}");
            transform.position = node.transform.position;
            _currentNode = node;
            _currentNode.Asign();
            transform.GetChild(0).gameObject.SetActive(true);
            if (_preset.IsCorrect())
            {
                _gears.CheckPuzzle();
            }
        }
        else
        {
            transform.SetParent(_scrollTransform);
            transform.localPosition = _startPos;
        }
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