using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BrokenElectricityStartBtn : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private TMP_Text _formulaText;
    [SerializeField] private LineRenderer _wireLine;

    public event Action OnEndDragEvent;

    public bool IsConnected => _isConnected;
    public bool IsSolved => _isSolved;

    private BrokenElectricityEndBtn _endBtn;
    private FormulaSO _formula;
    private Vector3 _startDragPosition;
    private Vector3 _currentMousePosition;
    private bool _isDragging = false;
    private bool _isSolved = false;
    private bool _isConnected = false;

    private void Start()
    {
        if (_wireLine == null)
        {
            _wireLine = gameObject.AddComponent<LineRenderer>();
            _wireLine.startWidth = 0.05f;
            _wireLine.endWidth = 0.01f;
            _wireLine.positionCount = 2;
        }

        _startDragPosition = transform.position;
        _wireLine.SetPosition(0, _startDragPosition);
        _wireLine.SetPosition(1, _startDragPosition);
    }

    public void SetProperties(BrokenElectricityEndBtn endBtn, FormulaSO formula)
    {
        _endBtn = endBtn;
        _formula = formula;

        _formulaText.text = _formula.FirstPart;
        _endBtn.SetFormula(_formula.SecondPart);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isSolved || _isConnected) return;
        
        _isDragging = true;
        _startDragPosition = transform.position;
        _wireLine.SetPosition(0, _startDragPosition);
        _wireLine.SetPosition(1, _startDragPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        if (_isSolved || _isConnected) return;

        _currentMousePosition = GetMouseWorldPosition();
        _wireLine.SetPosition(1, _currentMousePosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isSolved || _isConnected) return;

        _isDragging = false;

        GameObject hitObject = GetObjectUnderMouse();
        if (hitObject != null && hitObject.TryGetComponent(out BrokenElectricityEndBtn endBtn))
        {
            Debug.Log("Соединение успешно!");
            _wireLine.SetPosition(1, hitObject.transform.position);
            _isConnected = true;
            if (endBtn.Formula == _endBtn.Formula)
                _isSolved = true;
        }

        OnEndDragEvent?.Invoke();
    }

    public void Reset()
    {
        Debug.Log("Соединение не удалось.");
        _wireLine.SetPosition(1, _startDragPosition);
        _isSolved = false;
        _isConnected = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private GameObject GetObjectUnderMouse()
    {
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var result = new RaycastResult();
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            result = results[0];
            return result.gameObject;
        }

        return null;
    }
}