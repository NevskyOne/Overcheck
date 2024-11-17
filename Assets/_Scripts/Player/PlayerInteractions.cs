using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractions : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _clickMask;
    
    [Header("Cursors")] 
    [SerializeField] private GameObject _cursor;
    [SerializeField] private Image _cursorImg;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _NPCSprite;
    [SerializeField] private Sprite _BedSprite;    
    [SerializeField] private Sprite _RadioSprite;  
    [SerializeField] private Sprite _UISprite;

    [Header("UI")] 
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Transform _objectHolder;
    [SerializeField] private DragRotate _rotateScript;
    
    private PlayerInput _playerInput => GetComponent<PlayerInput>();
    private PlayerMovement _playerMove => GetComponent<PlayerMovement>();
    private DialogSystem _dialogSystem;
    private bool _inDialog;
    
    private void Start()
    {
        _playerInput.actions["Click"].started += OnClick;
        _playerInput.actions["Space"].performed += Space;
        _playerInput.actions["Escape"].performed += Escape;

        Focus();
        _dialogSystem = FindFirstObjectByType<DialogSystem>();
        _dialogSystem.ChatEnded += () => {_inDialog = false; Focus();};
    }

    private void Update()
    {
        
        if (_inDialog ||
            !Physics.Raycast(_camera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0)),
                out var hit, 4, _clickMask))
            _cursorImg.sprite = _defaultSprite;
        else
        {
            if (hit.transform.CompareTag("NPC"))
            {
                _cursorImg.sprite = _NPCSprite;
            }
            else if (hit.transform.CompareTag("Bed"))
            {
                _cursorImg.sprite = _BedSprite;
            }
            else if (hit.transform.CompareTag("Radio"))
            {
                _cursorImg.sprite = _RadioSprite;
            }
            else if (hit.transform.CompareTag("OpenUI") || hit.transform.CompareTag("NPCObject"))
            {
                _cursorImg.sprite = _UISprite;
            }
        }
    }

    private void OnClick(InputAction.CallbackContext _)
    {
        if (_inDialog || 
            !Physics.Raycast(_camera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0)),
                out var hit, 4, _clickMask)) return;
        var transf = hit.transform;
        if (transf.CompareTag("NPC"))
        {
            StopFocus();

            transf.GetComponent<NPC>().StartChat();
            _inDialog = true;
        }
        else if (transf.CompareTag("NPCObject"))
        {
            StopFocus();
            transf.SetParent(_objectHolder);
            transf.localPosition = Vector3.zero;
            _rotateScript.EnableUI(transf.GetComponent<NPCObject>());
            _inDialog = true;
            _camera.cullingMask = LayerMask.GetMask("UI", "Clickable");
        }
        else if (transf.CompareTag("Radio"))
        {
        }
        else if (transf.CompareTag("OpenUI"))
        {
        }
    }

    private void Space(InputAction.CallbackContext _) => NextFraze();
    public void NextFraze()
    {
        _dialogSystem.PlayNext();
    }

    private void Escape(InputAction.CallbackContext _)
    {
        if (_inDialog)
        {
            _inDialog = false;
            _dialogSystem.EndChat();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (Time.timeScale == 0)
        {
            _pauseMenu.SetActive(false);
            Focus();
            Time.timeScale = 1;
        }
        else
        {
            _pauseMenu.SetActive(true);
            StopFocus();
            Time.timeScale = 0;
        }
    }

    public void Focus()
    {
        _inDialog = false;
        _playerMove.enabled = true;
        _cursor.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _camera.cullingMask = LayerMask.GetMask("Default", "UI", "Clickable");
    }
    
    private void StopFocus()
    {
        _playerMove.enabled = false;
        _cursor.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
