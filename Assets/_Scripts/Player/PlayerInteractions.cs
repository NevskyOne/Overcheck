using System.Threading.Tasks;
using Unity.VisualScripting;
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
    [SerializeField] private Sprite _bedSprite;    
    [SerializeField] private Sprite _radioSprite;
    [SerializeField] private Texture2D _correctSprite;
    [SerializeField] private Texture2D _wrongSprite;
    [SerializeField] private Texture2D _backDocSprite;
    [SerializeField] private Sprite _UISprite;

    [Header("UI")] 
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Transform _objectHolder;
    [SerializeField] private DragRotate _rotateScript;
    [SerializeField] private Transform _tableCamPos;
    [SerializeField] private GameObject _correctStamp;
    [SerializeField] private GameObject _wrongStamp;
    
    private PlayerInput _playerInput => GetComponent<PlayerInput>();
    private PlayerMovement _playerMove => GetComponent<PlayerMovement>();
    private TimeLines _timeLines => GetComponent<TimeLines>();
    private NPCManager _npcMng;
    private DialogSystem _dialogSystem;
    
    private bool _inDialog, _inTable, _isHolding;
    private CheckState _tableState = CheckState.None;
    private Transform _currentDoc;
    
    private void Start()
    {
        _playerInput.actions["Click"].started += OnClick;
        _playerInput.actions["Click"].canceled += OnClickEnd;
        _playerInput.actions["Space"].performed += Space;
        _playerInput.actions["Escape"].performed += Escape;

        Focus();
        _dialogSystem = FindFirstObjectByType<DialogSystem>();
        _npcMng = FindFirstObjectByType<NPCManager>();
        _dialogSystem.ChatEnded += () => {_inDialog = false; Focus();};
    }

    private void Update()
    {
        RaycastHit hit = new ();
        if (_inDialog ||
            !Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out hit, 4, _clickMask))
            _cursorImg.sprite = _defaultSprite;
        else
        {
            if (hit.transform.CompareTag("NPC"))
                _cursorImg.sprite = _NPCSprite;
            else if (hit.transform.CompareTag("Bed"))
                _cursorImg.sprite = _bedSprite;
            else if (hit.transform.CompareTag("Radio"))
                _cursorImg.sprite = _radioSprite;
            // else if (_inTable && hit.transform.CompareTag("Correct"))
            //     Cursor.SetCursor(_correctSprite, Vector2.zero, CursorMode.ForceSoftware);
            // else if (_inTable && hit.transform.CompareTag("Wrong"))
            //     Cursor.SetCursor(_wrongSprite, Vector2.zero, CursorMode.ForceSoftware);
            // else if (_inTable && hit.transform.CompareTag("BackDoc"))
                // Cursor.SetCursor(_backDocSprite, Vector2.zero, CursorMode.ForceSoftware);
            else if (!hit.transform.CompareTag("Untagged"))
                _cursorImg.sprite = _UISprite;
        }

        if (_isHolding && _currentDoc && hit.transform && !hit.transform.CompareTag("Document"))
        {
            _currentDoc.localPosition = new Vector3(hit.point.x,hit.point.y + 0.4f,hit.point.z);
        }
    }

    private async void OnClick(InputAction.CallbackContext _)
    {
        if (_inDialog || 
            !Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                out var hit, 4, _clickMask)) return;
        var transf = hit.transform;
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
            _camera.cullingMask = LayerMask.GetMask("UI", "Clickable");
        }
        else if (transf.CompareTag("Bed"))
        {
            _timeLines.Sleep();
        }
        else if (!_inTable && transf.CompareTag("Table"))
        {
            StopFocus();
            _camera.transform.parent = _tableCamPos;
            _camera.transform.localPosition = Vector3.zero;
            _camera.transform.localRotation = Quaternion.Euler(90,0,0);
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
        else if (_inTable && transf.CompareTag("Document"))
        {
            if (_tableState == CheckState.Correct && transf.TryGetComponent<PMSDocument>(out var _))
            {
                Instantiate(_correctStamp, transf.GetChild(0));
                _npcMng.NPCCheck(Goal.Other,true);/////////Update
            }
            else if (_tableState == CheckState.Wrong && transf.TryGetComponent<PMSDocument>(out var _))
            {
                Instantiate(_wrongStamp, transf.GetChild(0));
                _npcMng.NPCCheck(Goal.Other,false);/////////Update
            }
            else
            {
                _isHolding = true;
                _currentDoc = transf;
                _currentDoc.localPosition = new Vector3(_currentDoc.localPosition.x, _currentDoc.localPosition.y+0.4f, _currentDoc.localPosition.z);
            }
        }
        else if (_inTable && hit.transform.CompareTag("BackDoc"))
        {
            await Task.Delay(500);
            _npcMng.NPCCollectDoc();
        }
        else if (_inTable && transf.CompareTag("Radio"))
        {
        }
        else if (transf.CompareTag("OpenUI"))
        {
        }
    }
    

    private void OnClickEnd(InputAction.CallbackContext _)
    {
        _isHolding = false;
        _currentDoc = null;
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
        else if (_inTable)
        {
            if (_tableState == CheckState.None)
            {
                _inTable = false;
                _camera.transform.parent = transform;
                _camera.transform.localPosition = new Vector3(0, 0.64f, 0);
                _camera.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Focus();
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
        _camera.cullingMask = LayerMask.GetMask("Default", "UI", "Clickable");
    }
    
    public void StopFocus()
    {
        _playerMove.enabled = false;
        _cursor.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

public enum CheckState
{
    None,
    Wrong,
    Correct
}
