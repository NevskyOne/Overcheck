using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Player : MonoBehaviour
{
    [Header("Structures")] 
    [SerializeField] private MonologueStruct _monologueStruct;
    [SerializeField] private MovementStruct _movementStruct;
    [Header("Layer Masks")]
    [SerializeField] private LayerMask _clickMask;
    [SerializeField] private LayerMask _docsMask;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _breathSource;
    [SerializeField] private AudioSource _feetSource;
    [Header("Public objects")]
    [field: SerializeReference] public Camera Cam { get; private set; }
    [field: SerializeReference] public DragRotate Rotator { get; private set; }
    [field: SerializeReference] public GameObject Model { get; private set; }
    [Header("UI")]
    [SerializeField] private List<TMP_Text> _honeyCombsTextes;

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
    
    [Inject]
    private void Initialize(VisualEffects effects, DialogSystem dialogSystem)
    {
        _effects = effects;
        _dialogSystem = dialogSystem;

        Input = GetComponent<PlayerInput>();
        Monologue = new PlayerMonologue(_dialogSystem,_monologueStruct);
        Sfx = new PlayerSFX(_breathSource, _feetSource);
        Coins = new PlayerCoins(_honeyCombsTextes);
        Movement = new PlayerMovement(_movementStruct,_effects,Cam,transform);
        Interactions = new PlayerInteractions(_clickMask, _docsMask, Cam, Rotator);
    }

    private void FixedUpdate()
    {
        Movement.LocalUpdate();
    }
}

public enum PlayerState
{
    Movement,
    Dialog,
    CamSwitcher,
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
