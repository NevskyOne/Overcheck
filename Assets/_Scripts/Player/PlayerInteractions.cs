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
    
    private void StartSprint(InputAction.CallbackContext _ = new InputAction.CallbackContext())
    {
        if(Player.State != PlayerState.Movement && Player.State != PlayerState.Holding) return;
        Player.Movement.StartSprint();
    }
    
    public void StopSprint(InputAction.CallbackContext _ = new InputAction.CallbackContext())
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
            case PlayerState.Movement or PlayerState.Block:
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

