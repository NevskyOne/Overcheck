using TMPro;
using UnityEngine;

public class DragRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 0.2f;  // Чувствительность вращения
    [SerializeField] private Vector2 rotationXLimits = new (-45f, 45f); // Ограничение по оси X

    [Header("Inertia Settings")]
    [SerializeField] private float inertiaDuration = 1f; // Время затухания инерции

    [Header("UI")]
    [SerializeField] private GameObject _worldCanvas;
    [SerializeField] private TMP_Text _objName;
    [SerializeField] private TMP_Text _objDescript;
    
    private Vector2 _currentVelocity;       // Текущая скорость вращения
    private Vector2 _inertiaVelocity;       // Скорость для инерции
    private float _inertiaTimeRemaining;    // Время оставшейся инерции

    private bool _isDragging;       // Флаг нажатия мыши
    private Transform _targetTransform;     // Цель вращения
    public Movable TargetMovable { get; private set; }
    
    private void OnEnable()
    {
        _worldCanvas.SetActive(true);
        _targetTransform = transform.GetChild(0);
        TargetMovable = _targetTransform.GetComponent<Movable>();
        _objName.text = TargetMovable.Title;
        _objDescript.text = TargetMovable.Description;
        
        enabled = true;
    }
    
    private void OnDisable()
    {
        _worldCanvas.SetActive(false);
        _targetTransform.SetParent(null);
        TargetMovable.Uninteract();
        enabled = false;
    }
    
    // Вызывается из Player Input при движении мыши
    public void OnLook(Vector2 delta)
    {
        if (_isDragging)
        {
            Rotate(delta);
        }
    }

    // Вызывается из Player Input при нажатии кнопки мыши
    public void OnPointerDown()
    {
        _isDragging = true;
        _inertiaTimeRemaining = 0; // Сброс инерции при новом перетаскивании
    }

    // Вызывается из Player Input при отпускании кнопки мыши
    public void OnPointerUp()
    {
        _isDragging = false;
        _inertiaTimeRemaining = inertiaDuration;
        _inertiaVelocity = _currentVelocity; // Запоминаем текущую скорость вращения
    }

    private void Update()
    {
        // Если не драг, применяем инерцию
        if (!_isDragging && _inertiaTimeRemaining > 0)
        {
            float t = _inertiaTimeRemaining / inertiaDuration;
            Vector2 delta = _inertiaVelocity * t * Time.deltaTime;
            Rotate(delta);
            _inertiaTimeRemaining -= Time.deltaTime;
        }
    }

    private void Rotate(Vector2 delta)
    {
        // Рассчитываем вращение
        float rotationX = delta.y * rotationSpeed;
        float rotationY = -delta.x * rotationSpeed;

        // Применяем вращение
        Vector3 currentEuler = _targetTransform.eulerAngles;
        float newRotationX = Mathf.Clamp(NormalizeAngle(currentEuler.x + rotationX), rotationXLimits.x, rotationXLimits.y);
        float newRotationY = currentEuler.y + rotationY;

        // Применяем новые углы
        _targetTransform.eulerAngles = new Vector3(newRotationX, newRotationY, 0);

        // Сохраняем текущую скорость для инерции
        _currentVelocity = delta;
    }

    private float NormalizeAngle(float angle)
    {
        // Нормализация угла в диапазон [-180, 180]
        angle = angle % 360;
        return angle > 180 ? angle - 360 : angle;
    }
}
