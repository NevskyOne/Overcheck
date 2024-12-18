using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractions : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _clickMask;
    [SerializeField] private LayerMask _docMask;
    [SerializeField] private LayerMask _docPlace;
    [Header("CamMoves")]
    [SerializeField] private CamMove _tableMove;
    [SerializeField] private CamMove _leftScreenMove;
    [SerializeField] private CamMove _rightScreenMove;
    [Header("Cursors")] 
    [SerializeField] private GameObject _cursor;
    [SerializeField] private Image _cursorImg;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _NPCSprite;
    [SerializeField] private Sprite _bedSprite;    
    [SerializeField] private Sprite _radioSprite;
    [SerializeField] private Sprite _startSprite;
    [SerializeField] private Sprite _UISprite;
    [Header("UI")] 
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Transform _objectHolder;
    [SerializeField] private DragRotate _rotateScript;
    [SerializeField] private Transform _tableCamPos;
    [SerializeField] private GameObject _correctStamp;
    [SerializeField] private GameObject _wrongStamp;
    [SerializeField] private GameObject _popupMenu;
    [SerializeField] private TMP_Text _popupText;
    [Header("Buttons")]
    [SerializeField] private Material _buttonMaterial;
    [SerializeField] private Material _corectMaterial;
    [SerializeField] private Material _wrongMaterial;
    
    public static LayerMask DefaultMask;
    
    private PlayerInput _playerInput => GetComponent<PlayerInput>();
    private PlayerMovement _playerMove => GetComponent<PlayerMovement>();
    private TimeLines _timeLines => GetComponent<TimeLines>();
    private CameraManager _camManager => FindFirstObjectByType<CameraManager>();
    private NPCManager _npcMng => FindFirstObjectByType<NPCManager>();
    private DialogSystem _dialogSystem => FindFirstObjectByType<DialogSystem>();

    private bool  _isHolding, _canSleep, _canStartDay = true;
    private CheckState _tableState = CheckState.None;
    public static PlayerState PlayerState { get; set; } = PlayerState.None;
    private Transform _currentDoc;

    private void Start()
    {
        SettingsUI.ChangeVFX(SettingsUI.VFXOn);
        
        _playerInput.actions["Click"].started += Click;
        _playerInput.actions["Click"].canceled += OnClickEnd;
        _playerInput.actions["RightClick"].started += RightClick;
        _playerInput.actions["Space"].performed += Space;
        _playerInput.actions["Escape"].performed += Escape;
        
        Focus();
        _dialogSystem.ChatEnded += () => {PlayerState = PlayerState.None; Focus();};
        NPCManager.OnNPCEnd += () => _canSleep = true;
        
        TimeLines.OnDayEnd += () =>
        {
            _canStartDay = true;
            _camManager.ResetCamera();
            _buttonMaterial.color = Color.green;
            transform.GetChild(1).gameObject.SetActive(true);
        };
        _buttonMaterial.color = Color.green;
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        switch (_tableState)
        {
            case CheckState.Correct:
                _corectMaterial.color = Color.green;
                _wrongMaterial.color = Color.grey;
                break;
            case CheckState.Wrong:
                _wrongMaterial.color = Color.red;
                _corectMaterial.color = Color.grey;
                break;
            case CheckState.None:
                _wrongMaterial.color =  Color.grey;
                _corectMaterial.color = Color.grey;
                break;
        }

        var delta = _playerInput.actions["Move"].ReadValue<Vector2>();
        switch (PlayerState)
        {
            case PlayerState.Table when delta.x < 0:
                _leftScreenMove.Move(transform.eulerAngles);
                PlayerState = PlayerState.LeftScreen;
                break;
            case PlayerState.Table when delta.x > 0:
                _rightScreenMove.Move(transform.eulerAngles);
                PlayerState = PlayerState.RightScreen;
                break;
            case PlayerState.LeftScreen when delta is { y: < 0, x: 0 }:
                _tableMove.Move(transform.eulerAngles);
                PlayerState = PlayerState.Table;
                break;
            case PlayerState.LeftScreen when delta is { x: > 0, y: 0 }:
                _rightScreenMove.Move(transform.eulerAngles);
                PlayerState = PlayerState.RightScreen;
                break;
            case PlayerState.RightScreen when delta is { x: < 0, y: 0 }:
                _leftScreenMove.Move(transform.eulerAngles);
                PlayerState = PlayerState.LeftScreen;
                break;
            case PlayerState.RightScreen when delta is { y: < 0, x: 0 }:
                _tableMove.Move(transform.eulerAngles);
                PlayerState = PlayerState.Table;
                break;
        }
        
        _popupMenu.gameObject.SetActive(false);
        RaycastHit hit = new (), hit2 = new();
        
        if (PlayerState == PlayerState.Dialog || !Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out hit, 3, _clickMask))
        {
            _cursorImg.sprite = _defaultSprite;
        }
        else
        {
            if (hit.transform.CompareTag("NPC"))
                _cursorImg.sprite = _NPCSprite;
            else if (_canSleep && hit.transform.CompareTag("Bed"))
                _cursorImg.sprite = _bedSprite;
            else if (hit.transform.CompareTag("Radio"))
                _cursorImg.sprite = _radioSprite;
            else if (_canStartDay && hit.transform.CompareTag("StartDay"))
                _cursorImg.sprite = _startSprite;
            else if (PlayerState == PlayerState.Table && hit.transform.CompareTag("Correct"))
            {
                _popupText.text = "Разрешить";
                _popupMenu.gameObject.SetActive(true);
            }
            else if (PlayerState == PlayerState.Table && hit.transform.CompareTag("Wrong"))
            {
                _popupText.text = "Не пустить";
                _popupMenu.gameObject.SetActive(true);
            }
            else if (!hit.transform.CompareTag("Untagged") && ! hit.transform.CompareTag("Bed") && !hit.transform.CompareTag("StartDay"))
                _cursorImg.sprite = _UISprite;
        }

        if (_isHolding && _currentDoc && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out hit2, 3,_docPlace))
        {
            _currentDoc.localPosition = new Vector3(hit2.point.x,_currentDoc.localPosition.y,hit2.point.z);
        }
    }

    private async void Click(InputAction.CallbackContext _)
    {
        if (Time.timeScale == 0 || PlayerState == PlayerState.Dialog) return;
        
        Transform transf;
        if (PlayerState == PlayerState.Table && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out var hit, 3, _docMask))
        {
            transf = hit.transform;
            if (_tableState != CheckState.None 
                && transf.CompareTag("PMS")
                && transf.GetChild(0).GetChild(0).childCount == 0)
            {
                Instantiate(_tableState == CheckState.Correct? _correctStamp : _wrongStamp,
                    transf.GetChild(0).GetChild(0));
                NPCManager.CurrentNPC.Check(_tableState == CheckState.Correct);
                _tableState = CheckState.None;
            }
            else
            {
                _isHolding = true;
                _currentDoc = transf;
                _currentDoc.GetComponent<Rigidbody>().useGravity = false;
                _currentDoc.localPosition = new Vector3(_currentDoc.localPosition.x, hit.point.y+0.2f, _currentDoc.localPosition.z);
            }
        }
        
        if (!Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out var hit2, 3, _clickMask)) return;
        
        transf = hit2.transform;
        
        if (PlayerState == PlayerState.None && transf.CompareTag("NPC"))
        {
            transf.GetComponent<NPC>().StartChat();
            PlayerState = PlayerState.Dialog;
        }
        else if (PlayerState == PlayerState.None && transf.CompareTag("NPCObject"))
        {
            StopFocus();
            transf.SetParent(_objectHolder);
            transf.localPosition = Vector3.zero;
            _rotateScript.EnableUI(transf.GetComponent<NPCObject>());
            PlayerState = PlayerState.UI;
            _camera.cullingMask = LayerMask.GetMask("UI", "NPCObject");
        }
        else if (_canSleep && transf.CompareTag("Bed"))
        {
            StopFocus();
            _canSleep = false;
            transf.GetComponent<CamMove>().Move(transform.eulerAngles);
            
            transform.GetChild(1).gameObject.SetActive(false);
                
            await Task.Delay(2000);
            _timeLines.Sleep();
        }
        else if (_canStartDay && hit2.transform.CompareTag("StartDay"))
        {
            _npcMng.StartDay();
            _canStartDay = false;
            _buttonMaterial.color = Color.gray;
        }
        else if (PlayerState == PlayerState.None && transf.CompareTag("Table"))
        {
            StopFocus();
            _tableMove.Move(transform.eulerAngles);
            PlayerState = PlayerState.Table;
        }
        else if (PlayerState == PlayerState.Table && transf.CompareTag("Correct"))
        {
            _tableState = CheckState.Correct;
        }
        else if (PlayerState == PlayerState.Table && transf.CompareTag("Wrong"))
        {
            _tableState = CheckState.Wrong;
        }
        else if (transf.CompareTag("Radio"))
        {
            var audioSource = transf.GetComponent<AudioSource>();
            audioSource.mute = !audioSource.mute;
            transf.GetComponent<Radio>().RadioMat.color = audioSource.mute ? Color.red : Color.green;
        }
        else if (PlayerState == PlayerState.None && transf.CompareTag("OpenUI"))
        {
            StopFocus();
            if(transf.name == "Criminals")
                PlayerState = PlayerState.LeftScreen;
            else if(transf.name == "Tablet")
                PlayerState = PlayerState.RightScreen;
            else
                PlayerState = PlayerState.UI;
            transf.GetComponent<CamMove>().Move(transform.eulerAngles);
        }
    }
    

    private void OnClickEnd(InputAction.CallbackContext _)
    {
        _isHolding = false;
        if (_currentDoc != null)
        {
            _currentDoc.GetComponent<Rigidbody>().useGravity = true;
            _currentDoc = null;
        }
    }

    private void RightClick(InputAction.CallbackContext _)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out var hit, 4, _clickMask) && hit.transform.CompareTag("Radio"))
        {
            hit.transform.GetComponent<Radio>().ChangeClip();
        }
    }

    private void Space(InputAction.CallbackContext _)
    {
        if(PlayerState == PlayerState.Dialog)
            _dialogSystem.PlayNext();
    }

    public void ResetButton() => _canStartDay = true;
    


    private void Escape(InputAction.CallbackContext _)
    {
        if (PlayerState == PlayerState.Dialog)
        {
            _dialogSystem.EndChat();
            PlayerState = PlayerState.None;
        }
        else if (PlayerState is PlayerState.UI or PlayerState.LeftScreen or PlayerState.RightScreen)
        {
            _rotateScript.DisableUI();
            _camManager.ResetCamera();
        }
        else if (PlayerState == PlayerState.Table)
        {
            _camManager.ResetCamera();
            _tableState = CheckState.None;
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
        PlayerState = PlayerState.None;
        _playerMove.enabled = true;
        _cursor.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _camera.cullingMask = DefaultMask;
    }
    
    public void StopFocus()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _cursor.SetActive(false);
        _playerMove.enabled = false;
    }
}

public enum CheckState
{
    None,
    Wrong,
    Correct
}

public enum PlayerState
{
    None,
    Dialog,
    Table,
    LeftScreen,
    RightScreen,
    UI
}
