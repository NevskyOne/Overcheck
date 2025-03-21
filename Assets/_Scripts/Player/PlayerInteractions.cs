using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions
{
    public CamSwitcher Switcher;
    
    private readonly LayerMask _clickMask,_docsMask,_docsPlaceMask;
    private readonly Camera _camera;
    private readonly DragRotate _dragRotate;
    private readonly MainUI _mainUI;
    private readonly DialogSystem _dialogSystem;
    private IInteractable _interactable, _doc;

    private bool _lockTheView;

    public PlayerInteractions(LayerMask clickMask, LayerMask docsMask, LayerMask docsPlaceMask, Camera cam,
                DragRotate dragRotate, MainUI mainUI, DialogSystem dialogSystem)
    {
        var playerInput = Player.Input;
        _clickMask = clickMask;
        _docsMask = docsMask;
        _docsPlaceMask = docsPlaceMask;
        _camera = cam;
        _dragRotate = dragRotate;

        playerInput.actions["Click"].started += Click;
        playerInput.actions["Click"].canceled += OnClickEnd;
        playerInput.actions["MiddleClick"].started += MiddleClick;
        playerInput.actions["MiddleClick"].canceled += MiddleClickEnd;
        playerInput.actions["Use"].started += Use;
        playerInput.actions["Move"].started += Move;
        playerInput.actions["Look"].performed += Look;
        playerInput.actions["Sprint"].started += StartSprint;
        playerInput.actions["Sprint"].canceled += StopSprint;
        playerInput.actions["Space"].performed += Space;
        playerInput.actions["Escape"].performed += Escape;
        
        _mainUI = mainUI;
        _dialogSystem = dialogSystem;
        Focus();
    }

    private void Click(InputAction.CallbackContext _)
    {
        switch (Player.State)
        {
            case PlayerState.Movement:
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                        out var hit, 3, _clickMask) &&
                        hit.transform.TryGetComponent<IInteractable>(out var iter))
                {
                    if (hit.transform.TryGetComponent<CamMove>(out var camMove))
                        camMove.HitPos = hit.point;
                    _interactable = iter;
                    _interactable.Interact();
                }
                break;
            case PlayerState.Checking:
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                        out var hit2, 3, _docsMask))
                {
                    switch (Player.CheckingState)
                    {
                        case CheckState.None:
                            _doc = hit2.transform.GetComponent<IInteractable>();
                            
                            Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                                out var hit3, 3, _docsPlaceMask);
                            var trueDoc = _doc as Document;
                            trueDoc?.SetInitialHeight(hit3.point.y);
                            
                            _doc.Interact();
                            break;
                        case CheckState.Correct:
                            if (hit2.transform.TryGetComponent<IAcceptable>(out var acceptable))
                            {
                                acceptable.Accept();
                            }
                            break;
                        case CheckState.Wrong:
                            if (hit2.transform.TryGetComponent<IAcceptable>(out var acceptable2))
                            {
                                acceptable2.Reject();
                            }
                            break;
                    }
                }
                else if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), 
                        out var hit4, 3, _clickMask) && 
                        hit4.transform.TryGetComponent<StampButton>(out var stamp))
                {
                    stamp.Interact();
                }
                break;
        }
    }

    private void MiddleClick(InputAction.CallbackContext _)
    {
        if (Player.State == PlayerState.Holding)
        {
            _dragRotate.OnPointerDown();
            _lockTheView = true;
        }
    }
    
    private void MiddleClickEnd(InputAction.CallbackContext _)
    {
        if (Player.State == PlayerState.Holding && Player.CheckingState == CheckState.None)
        {
            _dragRotate.OnPointerUp();
            _lockTheView = false;
        }
    }

    private void OnClickEnd(InputAction.CallbackContext _)
    {
        if (Player.State == PlayerState.Checking){
            _doc?.Uninteract();
            _doc = null;
        }
    }
    
    private void Use(InputAction.CallbackContext _)
    {
        switch (Player.State)
        {
            case PlayerState.Movement:
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), 
                        out var hit, 3, _clickMask) &&
                        hit.transform.TryGetComponent<IUsable>(out var usable))
                    usable.Use();
                break;
            case PlayerState.Holding:
                if (_dragRotate.TargetMovable.TryGetComponent<IUsable>(out var usable2))
                {
                    _dragRotate.enabled = false;
                    usable2.Use();
                }
                break;
        }
    }
    
    private void Move(InputAction.CallbackContext ctx)
    {
        switch (Player.State)
        {
            case PlayerState.UI or PlayerState.Checking:
                Switcher?.SwitchCamMove(ctx.ReadValue<Vector2>().normalized);
                break;
        }
    }
    
    private void Look(InputAction.CallbackContext ctx)
    {
        var delta = ctx.ReadValue<Vector2>();
        switch (Player.State)
        {
            case PlayerState.Movement:
                Player.Movement.Look(delta);
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                        out var hit, 3, _clickMask)&&
                    hit.transform.TryGetComponent<IInteractable>(out var iter))
                {
                    _mainUI.ChangeCursor(iter.CursorInd);
                }
                else
                {
                    _mainUI.ChangeCursor(0);
                }
                break;
            case PlayerState.Holding:
                if (_lockTheView)
                    _dragRotate.OnLook(delta);
                else
                    Player.Movement.Look(delta);
                break;
            case PlayerState.Checking:
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
                        out var hit2, 3, _docsPlaceMask))
                {
                    Document doc = _doc as Document;
                    doc?.Move(hit2.point);
                }
                break;
        }
    }
    
    private void StartSprint(InputAction.CallbackContext _)
    {
        if(Player.State != PlayerState.Movement && Player.State != PlayerState.Holding) return;
        Player.Movement.StartSprint();
    }
    
    private void StopSprint(InputAction.CallbackContext _)
    {
        if(Player.State != PlayerState.Movement && Player.State != PlayerState.Holding) return;
        Player.Movement.StopSprint();
    }
    
    private void Space(InputAction.CallbackContext _)
    {
        if(Player.State == PlayerState.Dialog)
            _dialogSystem.PlayNext();
    }
    
    private void Escape(InputAction.CallbackContext _)
    {
        switch (Player.State)
        {
            case PlayerState.Movement:
                PauseGame();
                break;
            case PlayerState.Dialog:
                _dialogSystem.EndChat();
                StopFocus();
                Player.State = PlayerState.Movement;
                break;
            case PlayerState.UI or PlayerState.Checking:
                _interactable?.Uninteract();
                _interactable = null;
                _doc?.Uninteract();
                _doc = null;
                Switcher = null;
                break;
            case PlayerState.Holding:
                _dragRotate.enabled = false;
                _interactable = null;
                break;
        }
    }
    
    public void PauseGame()
    {
        if (Time.timeScale == 0)
        {
            _mainUI.CloseMenus();
            Focus();
            Time.timeScale = 1;
            Player.State = PlayerState.Movement;
        }
        else
        { 
            _mainUI.Pause();
            StopFocus();
            Time.timeScale = 0;
            Player.State = PlayerState.Block;
        }
    }
    
    public void Focus()
    { 
        Player.Movement.Enable();
        _mainUI.ShowCursor();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
     
    public void StopFocus()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _mainUI.HideCursor();
        Player.Movement.Disable();
    }
}

//     private void Start()
//     {
//         _dialogSystem.ChatEnded += () => {PlayerState = PlayerState.None; Focus();};
//         NPCManager.OnNPCEnd += () => _canSleep = true;
//         
//         TimeLines.OnDayEnd += () =>
//         {
//             _camManager.ResetCamera();
//             transform.GetChild(1).gameObject.SetActive(true);
//             PlayerState = PlayerState.Dialog;
//         };
//     }
//
//     private void Update()
//     {
//         if (Time.timeScale == 0) return;
//
//         switch (_tableState)
//         {
//             case CheckState.Correct:
//                 _corectMaterial.color = Color.green;
//                 _wrongMaterial.color = Color.grey;
//                 break;
//             case CheckState.Wrong:
//                 _wrongMaterial.color = Color.red;
//                 _corectMaterial.color = Color.grey;
//                 break;
//             case CheckState.None:
//                 _wrongMaterial.color =  Color.grey;
//                 _corectMaterial.color = Color.grey;
//                 break;
//         }
//
//         var delta = _playerInput.actions["Move"].ReadValue<Vector2>();
//         switch (PlayerState)
//         {
//             case PlayerState.Table when delta.x < 0:
//                 _leftScreenMove.Move(transform.eulerAngles);
//                 PlayerState = PlayerState.LeftScreen;
//                 break;
//             case PlayerState.Table when delta.x > 0:
//                 _rightScreenMove.Move(transform.eulerAngles);
//                 PlayerState = PlayerState.RightScreen;
//                 break;
//             case PlayerState.LeftScreen when delta is { y: < 0, x: 0 }:
//                 _tableMove.Move(transform.eulerAngles);
//                 PlayerState = PlayerState.Table;
//                 break;
//             case PlayerState.LeftScreen when delta is { x: > 0, y: 0 }:
//                 _rightScreenMove.Move(transform.eulerAngles);
//                 PlayerState = PlayerState.RightScreen;
//                 break;
//             case PlayerState.RightScreen when delta is { x: < 0, y: 0 }:
//                 _leftScreenMove.Move(transform.eulerAngles);
//                 PlayerState = PlayerState.LeftScreen;
//                 break;
//             case PlayerState.RightScreen when delta is { y: < 0, x: 0 }:
//                 _tableMove.Move(transform.eulerAngles);
//                 PlayerState = PlayerState.Table;
//                 break;
//         }
//         
//         _mainUI.HidePopup();
//         RaycastHit hit = new (), hit2 = new();
//         
//         if (PlayerState == PlayerState.Dialog || !Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
//                 out hit, 3, _clickMask))
//         {
//             _mainUI.ChangeCursor(0);
//         }
//         else
//         {
//             if (hit.transform.CompareTag("NPC"))
//                 _mainUI.ChangeCursor(1);
//             else if (_canSleep && hit.transform.CompareTag("Bed"))
//                 _mainUI.ChangeCursor(2);
//             else if (hit.transform.CompareTag("Radio"))
//                 _mainUI.ChangeCursor(3);
//             else if (_button.Enabled && hit.transform.CompareTag("StartDay"))
//                 _mainUI.ChangeCursor(4);
//             else if (PlayerState == PlayerState.Table && hit.transform.CompareTag("Correct"))
//             {
//                 _mainUI.ShowPopup("Пустить");
//             }
//             else if (PlayerState == PlayerState.Table && hit.transform.CompareTag("Wrong"))
//             {
//                 _mainUI.ShowPopup("Не пустить");
//             }
//             else if (!hit.transform.CompareTag("Untagged") && ! hit.transform.CompareTag("Bed") && !hit.transform.CompareTag("StartDay"))
//                 _mainUI.ChangeCursor(5);
//         }
//
//         if (_isHolding && _currentDoc && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
//                 out hit2, 3,_docPlace))
//         {
//             _currentDoc.localPosition = new Vector3(hit2.point.x,_currentDoc.localPosition.y,hit2.point.z);
//         }
//     }
//
//     private async void Click(InputAction.CallbackContext _)
//     {
//         if (Time.timeScale == 0 || PlayerState == PlayerState.Dialog) return;
//         
//         Transform transf;
//         if (PlayerState == PlayerState.Table && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
//                 out var hit, 3, _docMask))
//         {
//             transf = hit.transform;
//             if (_tableState != CheckState.None 
//                 && transf.CompareTag("PMS")
//                 && transf.GetChild(0).GetChild(0).childCount == 0)
//             {
//                 Instantiate(_tableState == CheckState.Correct? _correctStamp : _wrongStamp,
//                     transf.GetChild(0).GetChild(0));
//                 NPCManager.CurrentNPC.Check(_tableState == CheckState.Correct);
//                 _tableState = CheckState.None;
//             }
//             else
//             {
//                 _isHolding = true;
//                 _currentDoc = transf;
//                 _currentDoc.GetComponent<Rigidbody>().useGravity = false;
//                 _currentDoc.localPosition = new Vector3(_currentDoc.localPosition.x, hit.point.y+0.2f, _currentDoc.localPosition.z);
//             }
//         }
//         
//         if (!Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
//                 out var hit2, 3, _clickMask)) return;
//         
//         transf = hit2.transform;
//         
//         if (PlayerState == PlayerState.None && transf.CompareTag("NPC"))
//         {
//             transf.GetComponent<NPC>().StartChat();
//             PlayerState = PlayerState.Dialog;
//         }
//         else if (PlayerState == PlayerState.None && transf.CompareTag("Movable"))
//         {
//             StopFocus();
//             _playerMove.enabled = true;
//             transf.GetComponent<Movable>().Take();
//             PlayerState = PlayerState.UI;
//         }
//         else if (_canSleep && transf.CompareTag("Bed"))
//         {
//             StopFocus();
//             _canSleep = false;
//             transf.GetComponent<CamMove>().Move(transform.eulerAngles);
//             
//             transform.GetChild(1).gameObject.SetActive(false);
//             PlayerState = PlayerState.Sleep;
//             await Task.Delay(2000);
//             _timeLines.Sleep();
//         }
//         else if (_button.Enabled && hit2.transform.CompareTag("StartDay"))
//         {
//             _npcMng.StartDay();
//             _button.Enabled = false;
//         }
//         else if (PlayerState == PlayerState.None && transf.CompareTag("Table"))
//         {
//             StopFocus();
//             _tableMove.Move(transform.eulerAngles);
//             PlayerState = PlayerState.Table;
//         }
//         else if (PlayerState == PlayerState.Table && transf.CompareTag("Correct"))
//         {
//             _tableState = CheckState.Correct;
//         }
//         else if (PlayerState == PlayerState.Table && transf.CompareTag("Wrong"))
//         {
//             _tableState = CheckState.Wrong;
//         }
//         else if (transf.CompareTag("Radio"))
//         {
//             var audioSource = transf.GetComponent<AudioSource>();
//             audioSource.mute = !audioSource.mute;
//             transf.GetComponent<Radio>().RadioMat.color = audioSource.mute ? Color.red : Color.green;
//         }
//         else if (PlayerState == PlayerState.None && transf.CompareTag("OpenUI"))
//         {
//             StopFocus();
//             if(transf.name == "Criminals")
//                 PlayerState = PlayerState.LeftScreen;
//             else if(transf.name == "Tablet")
//                 PlayerState = PlayerState.RightScreen;
//             else
//                 PlayerState = PlayerState.UI;
//             transf.GetComponent<CamMove>().Move(transform.eulerAngles);
//         }
//     }
//     
//
//     private void OnClickEnd(InputAction.CallbackContext _)
//     {
//         _isHolding = false;
//         if (_currentDoc != null)
//         {
//             _currentDoc.GetComponent<Rigidbody>().useGravity = true;
//             _currentDoc = null;
//         }
//     }
//
//     private void RightClick(InputAction.CallbackContext _)
//     {
//         if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),
//                 out var hit, 4, _clickMask) && hit.transform.CompareTag("Radio"))
//         {
//             hit.transform.GetComponent<Radio>().ChangeClip();
//         }
//     }
//
//     private void Space(InputAction.CallbackContext _)
//     {
//         if(PlayerState == PlayerState.Dialog)
//             _dialogSystem.PlayNext();
//     }
//
//     private void Escape(InputAction.CallbackContext _)
//     {
//         if (PlayerState == PlayerState.Dialog)
//         {
//             _dialogSystem.EndChat();
//             PlayerState = PlayerState.None;
//         }
//         else if (PlayerState is PlayerState.UI or PlayerState.LeftScreen or PlayerState.RightScreen)
//         {
//             _objectHolder.gameObject.SetActive(false);
//             _camManager.ResetCamera();
//         }
//         else if (PlayerState == PlayerState.Table)
//         {
//             _camManager.ResetCamera();
//             _tableState = CheckState.None;
//         }
//         else if (PlayerState == PlayerState.None)
//         {
//             PauseGame();
//         }
//     }
//
//     public void PauseGame()
//     {
//         if (Time.timeScale == 0)
//         {
//             _pauseMenu.SetActive(false);
//             Focus();
//             Time.timeScale = 1;
//         }
//         else
//         {
//             _pauseMenu.SetActive(true);
//             StopFocus();
//             Time.timeScale = 0;
//         }
//     }
//
//     public void Focus()
//     {
//         if(PlayerState == PlayerState.Dialog) return;
//         PlayerState = PlayerState.None;
//         _playerMove.enabled = true;
//         _mainUI.ShowCursor();
//         Cursor.visible = false;
//         Cursor.lockState = CursorLockMode.Locked;
//         _camera.cullingMask = DefaultMask;
//     }
//     
//     public void StopFocus()
//     {
//         Cursor.visible = true;
//         Cursor.lockState = CursorLockMode.None;
//         _mainUI.HideCursor();
//         _playerMove.enabled = false;
//     }
// }