using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class ReactorRod : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ReactorRodSlot CurrentSlot { get; private set; }
    public RodType RodType { get; set; }

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _parentCanvas;
    private Vector3 _startPosition;
    private Transform _originalParent;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _parentCanvas = GetComponentInParent<Canvas>();
        _originalParent = transform.parent;
        _startPosition = _rectTransform.localPosition;
    }

    public void Initialize(RodType rodType)
    {
        RodType = rodType;
        _rectTransform.localPosition = _startPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

        if (CurrentSlot != null)
        {
            CurrentSlot.ClearSlot();
            CurrentSlot = null;
        }

        transform.SetParent(_parentCanvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            _rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        if (CurrentSlot == null)
        {
            ReturnToStartPosition();
        }
    }

    public void SetSlot(ReactorRodSlot slot)
    {
        CurrentSlot = slot;
        transform.SetParent(slot.transform);
        _rectTransform.localPosition = Vector3.zero;
    }

    private void ReturnToStartPosition()
    {
        transform.SetParent(_originalParent);
        _rectTransform.localPosition = _startPosition;
    }

    public void ResetRod()
    {
        CurrentSlot = null;
        transform.SetParent(_originalParent);
        _rectTransform.localPosition = _startPosition;
        gameObject.SetActive(false);
    }
}