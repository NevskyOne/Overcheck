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
    
    private PlayerInput _playerInput => GetComponent<PlayerInput>();
    private PlayerMovement _playerMove => GetComponent<PlayerMovement>();
    private TimeLines _timeLines => GetComponent<TimeLines>();
    private CameraManager _camManager => FindFirstObjectByType<CameraManager>();
    private NPCManager _npcMng => FindFirstObjectByType<NPCManager>();
    private DialogSystem _dialogSystem => FindFirstObjectByType<DialogSystem>();

    private bool _inDialog, _inTable, _isHolding, _canSleep, _inUI, _canStartDay = true;
    private CheckState _tableState = CheckState.None;
    private Transform _currentDoc;
    
    private void Start()
    {
        _playerInput.actions["Click"].started += Click;
        _playerInput.actions["Click"].canceled += OnClickEnd;
        _playerInput.actions["RightClick"].started += RightClick;
        _playerInput.actions["Space"].performed += Space;
        _playerInput.actions["Escape"].performed += Escape;

        Focus();
        _dialogSystem.ChatEnded += () => {_inDialog = false; Focus();};
        _npcMng.OnNPCEnd += () => _canSleep = true;
        
        FindFirstObjectByType<TimeLines>().OnDayEnd += () =>
        {
            _canStartDay = true;
            _camManager.ResetCamera();
        };
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        
        if(_inTable)
            _popupMenu.gameObject.SetActive(false);
        RaycastHit hit = new ();
        if (_inDialog ||
            !Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out hit, 4, _clickMask))
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
            else if (_inTable && hit.transform.CompareTag("Correct"))
            {
                _popupText.text = "Разрешить";
                _popupMenu.gameObject.SetActive(true);
            }
            else if (_inTable && hit.transform.CompareTag("Wrong"))
            {
                _popupText.text = "Не пустить";
                _popupMenu.gameObject.SetActive(true);
            }
            else if (!hit.transform.CompareTag("Untagged") && ! hit.transform.CompareTag("Bed") && !hit.transform.CompareTag("StartDay"))
                _cursorImg.sprite = _UISprite;
        }

        if (_isHolding && _currentDoc && hit.transform)
        {
            _currentDoc.localPosition = new Vector3(hit.point.x,_currentDoc.localPosition.y,hit.point.z);
        }
    }

    private async void Click(InputAction.CallbackContext _)
    {
        if (Time.timeScale == 0) return;
        
        Transform transf;
        if (_inTable && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out var hit, 4, _docMask))
        {
            transf = hit.transform;
            if (_tableState == CheckState.Correct && transf.TryGetComponent<PMSDocument>(out var _))
            {
                Instantiate(_correctStamp, transf.GetChild(0).GetChild(1));
                _npcMng.CurrentNPC.Check(true);
            }
            else if (_tableState == CheckState.Wrong && transf.TryGetComponent<PMSDocument>(out var _))
            {
                Instantiate(_wrongStamp, transf.GetChild(0).GetChild(1));
                _npcMng.CurrentNPC.Check(false);
            }
            else
            {
                _isHolding = true;
                _currentDoc = transf;
                _currentDoc.GetComponent<Rigidbody>().useGravity = false;
                _currentDoc.localPosition = new Vector3(_currentDoc.localPosition.x, hit.point.y+0.2f, _currentDoc.localPosition.z);
            }
        }
        
        if (_inDialog || 
            !Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out var hit2, 4, _clickMask)) return;
        
        transf = hit2.transform;
        
        if (transf.CompareTag("NPC"))
        {
            StopFocus();

            transf.GetComponent<NPC>().StartChat();
            _inDialog = true;
        }
        else if (!_inTable && transf.CompareTag("NPCObject"))
        {
            StopFocus();
            transf.SetParent(_objectHolder);
            transf.localPosition = Vector3.zero;
            _rotateScript.EnableUI(transf.GetComponent<NPCObject>());
            _inDialog = true;
            _camera.cullingMask = LayerMask.GetMask("UI", "NPCObject");
        }
        else if (_canSleep && transf.CompareTag("Bed"))
        {
            StopFocus();
            _canSleep = false;
            var pos = transf.position;
            _camManager.MoveToTarget(new Vector3(pos.x, pos.y + 0.8f,pos.z -0.4f),
                new Vector3(0, -transform.eulerAngles.y, 0));
            await Task.Delay(2000);
            _timeLines.Sleep();
        }
        else if (_canStartDay && hit2.transform.CompareTag("StartDay"))
        {
            _npcMng.StartDay();
            _canStartDay = false;
        }
        else if (!_inTable && transf.CompareTag("Table"))
        {
            StopFocus();
            _camManager.MoveToTarget(transf.GetChild(0).position,
                new Vector3(90, -transform.eulerAngles.y, 0));
            _inTable = true;
        }
        else if (_inTable && transf.CompareTag("Correct"))
        {
            _tableState = CheckState.Correct;
        }
        else if (_inTable && transf.CompareTag("Wrong"))
        {
            _tableState = CheckState.Wrong;
        }
        else if (transf.CompareTag("Radio"))
        {
            var audioSource = transf.GetComponent<AudioSource>();
            audioSource.mute = !audioSource.mute;       
        }
        else if (transf.CompareTag("OpenUI"))
        {
            StopFocus();
            _inUI = true;
            transf.GetComponent<OpenUI>().Open(transform.eulerAngles);
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
        else if (_inUI)
        {
            _inUI = false;
            _camManager.ResetCamera();
        }
        else if (_inTable)
        {
            if (_tableState == CheckState.None)
            {
                _inTable = false;
                _camManager.ResetCamera();
            }
            else
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
        _inDialog = false;
        _playerMove.enabled = true;
        _cursor.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _camera.cullingMask = LayerMask.GetMask("Default", "UI", "Clickable", "Document", "NPCObject");
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
