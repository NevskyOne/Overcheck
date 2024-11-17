using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 0.2f;  // Чувствительность вращения
    [SerializeField] private Vector2 rotationXLimits = new (-45f, 45f); // Ограничение по оси X

    [Header("Inertia Settings")]
    [SerializeField] private float inertiaDuration = 1f; // Время затухания инерции

    [Header("Other")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _worldCanvas;
    [SerializeField] private TMP_Text _objName;
    [SerializeField] private TMP_Text _objDescript;
    
    private Vector2 _currentVelocity;       // Текущая скорость вращения
    private Vector2 _inertiaVelocity;       // Скорость для инерции
    private float _inertiaTimeRemaining;    // Время оставшейся инерции

    private bool _isDragging;       // Флаг нажатия мыши
    private Transform _targetTransform;     // Цель вращения
    
    
    private void OnEnable()
    {
        _targetTransform = transform.GetChild(0);
        _playerInput.actions["Look"].performed += OnLook;
        _playerInput.actions["CLick"].started += OnPointerDown;
        _playerInput.actions["CLick"].canceled += OnPointerUp;
    }
    
    private void OnDisable()
    {
        _playerInput.actions["Look"].performed -= OnLook;
        _playerInput.actions["CLick"].started -= OnPointerDown;
        _playerInput.actions["CLick"].canceled -= OnPointerUp;
    }

    public void EnableUI(NPCObject npcObject)
    {
        _worldCanvas.SetActive(true);
        _objName.text = npcObject.Name;
        _objDescript.text = npcObject.Description;
        enabled = true;
    }
    
    public void DisableUI()
    {
        _worldCanvas.SetActive(false);
        Destroy(_targetTransform.gameObject);
        enabled = false;
    }
    
    // Вызывается из Player Input при движении мыши
    private void OnLook(InputAction.CallbackContext ctx)
    {
        if (_isDragging)
        {
            Vector2 delta = ctx.ReadValue<Vector2>();
            Rotate(delta);
        }
    }

    // Вызывается из Player Input при нажатии кнопки мыши
    private void OnPointerDown(InputAction.CallbackContext ctx)
    {
        _isDragging = true;
        _inertiaTimeRemaining = 0; // Сброс инерции при новом перетаскивании
    }

    // Вызывается из Player Input при отпускании кнопки мыши
    private void OnPointerUp(InputAction.CallbackContext ctx)
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
