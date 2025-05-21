using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Player : MonoBehaviour
{
    [field: SerializeReference] public Camera Cam { get; private set; }
    [field: SerializeReference] public DragRotate Rotator { get; private set; }
    [field: SerializeReference] public GameObject Model { get; private set; }
    [Header("Structures")] 
    [SerializeField] private MonologueStruct _monologueStruct;
    [SerializeField] private MovementStruct _movementStruct;
    [Header("Layer Masks")]
    [SerializeField] private LayerMask _clickMask;
    [SerializeField] private LayerMask _docsMask;
    [SerializeField] private LayerMask _docsPlaceMask;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _breathSource;
    [SerializeField] private AudioSource _feetSource;
    [Header("UI")]
    [SerializeField] private TMP_Text _dayCombsTextes;
    [SerializeField] private List<TMP_Text> _honeyCombsTextes;
    [SerializeField] private ScreenFade _screenFade;

    [Header("Transforms")] 
    [SerializeField] private Transform _bedTransform;
    [SerializeField] private Vector3 _initCamRot;

    public static PlayerState State { get; set; } = PlayerState.Movement;
    public static CheckState CheckingState { get; set; } = CheckState.None;
    
    public static PlayerInput Input { get; private set; }
    public static PlayerSFX Sfx  { get; private set; }
    public static PlayerMonologue Monologue { get; private set; }
    public static PlayerInteractions Interactions { get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCoins Coins { get; private set; }
    
    private VisualEffects _effects;
    private DialogSystem _dialogSystem;
    private Rigidbody _camRb => Cam.GetComponent<Rigidbody>();

    [Inject] private CameraManager _cameraManager;
    [Inject] private DaysService _daysService;
    
    [Inject]
    public void Initialize(VisualEffects effects, MainUI mainUI, DialogSystem dialogSystem, EventBus eventBus)
    {
        _effects = effects;
        _dialogSystem = dialogSystem;

        Input = GetComponent<PlayerInput>();
        Monologue = new PlayerMonologue(_dialogSystem,_monologueStruct);
        Sfx = new PlayerSFX(_breathSource, _feetSource);
        Coins = new PlayerCoins(_honeyCombsTextes,_dayCombsTextes, eventBus);

        Movement = new PlayerMovement(_movementStruct,_effects,Cam,transform, mainUI, eventBus);

        Interactions = new PlayerInteractions(_clickMask, _docsMask, _docsPlaceMask, Cam, Rotator, mainUI,_dialogSystem);
    }

    private async void Start()
    {
        await Task.Delay(10);
        transform.rotation = new Quaternion(0,_initCamRot.y,0,-0.1979992836713791f);
        Cam.transform.rotation = new Quaternion(_initCamRot.x,0,_initCamRot.z,0);
    }

    private void FixedUpdate()
    {
        Movement.LocalUpdate(Input.actions["Move"].ReadValue<Vector2>());
    }

    public async void Die()
    {
        _camRb.useGravity = true;
        _camRb.isKinematic = false;
        _screenFade.gameObject.SetActive(true);
        Model.SetActive(false);
        State = PlayerState.Block;
        await Task.Delay(4500);
        
        _camRb.isKinematic = true;
        _camRb.useGravity = false;
        _cameraManager.transform.localEulerAngles = new Vector3(0, 0, 0);
        _cameraManager.ResetCamera();
        transform.position = _bedTransform.position;
        Model.SetActive(true);
        StartCoroutine(_screenFade.EndFade());
        
        State = PlayerState.Movement;
        _daysService.StartNewDay(DaysService.CurrentDay);
    }
}

public enum PlayerState
{
    Movement,
    Dialog,
    Holding,
    Checking,
    UI,
    Block
}

public enum CheckState
{
    None,
    Wrong,
    Correct
}

[Serializable]
public struct MovementStruct
{
    [Header("Shake Settings")]
    [Range(0,5f)] public float ShakeAmplitude; 
    [Range(0,5f)] public float ShakeFrequency;
    [Header("Settings")]
    [Range(0,1f)] public float SmoothTime; 
    [Range(0,1f)] public float TransitionTime;
    public float MaxSpeed;
    public Vector2 RotationLimits;
    [Header("Animation")] public Animator Anim;
}

[Serializable]
public struct MonologueStruct
{
    public List<string> DayStartPhrases;
    public List<string> DayEndPhrases;
    public List<string> EventStartPhrases;
    public List<string> EventDonePhrases;
    public List<string> StrangeVoicePhrases;
    public List<string> EternityPhrases;
}
