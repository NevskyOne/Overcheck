using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BrokenElectricityStartBtn : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private TMP_Text _formulaText;
    [SerializeField] private LineRenderer _wireLine;
    [SerializeField] private float _zPos = 0.985f;
    public event Action OnEndDragEvent;

    public bool IsConnected => _isConnected;
    public bool IsSolved => _isSolved;

    private BrokenElectricityEndBtn _endBtn;
    private FormulaSO _formula;
    private Vector3 _currentMousePosition;
    private bool _isDragging;
    private bool _isSolved;
    private bool _isConnected;

    private void Start()
    {
        
        _wireLine.SetPosition(0, Vector3.zero);
        _wireLine.SetPosition(1, Vector3.zero);
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
        _wireLine.SetPosition(1, GetMouseWorldPosition());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        if (_isSolved || _isConnected) return;
        
        _wireLine.SetPosition(1, GetMouseWorldPosition());
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isSolved || _isConnected) return;

        _isDragging = false;

        GameObject hitObject = GetObjectUnderMouse();
        if (hitObject != null && hitObject.TryGetComponent(out BrokenElectricityEndBtn endBtn))
        {
            Debug.Log("Соединение успешно!");
            _wireLine.SetPosition(1, _wireLine.transform.InverseTransformPoint(endBtn.LinePos.position));
            endBtn.VFX.SetActive(true);
            _isConnected = true;
            
            if (endBtn.Formula == _endBtn.Formula)
                _isSolved = true;
        }

        OnEndDragEvent?.Invoke();
    }

    public void Reset()
    {
        Debug.Log("Соединение не удалось.");
        _wireLine.SetPosition(1, Vector3.zero);
        _isSolved = false;
        _isConnected = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = _zPos;
        
        var worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        var localPos = _wireLine.transform.InverseTransformPoint(worldPos);
        return new Vector3(localPos.x, localPos.y, 0);
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