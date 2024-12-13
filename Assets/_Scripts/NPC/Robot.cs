
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class Robot : MonoBehaviour
{
    private static readonly int FresnelColor = Shader.PropertyToID("_FresnelColor");

    [Header("Transforms")]
    [SerializeField] private Transform _player;
    [SerializeField] private List<Transform> _positions;

    [Header("Parameters")]
    [SerializeField] private float _huntSpeed;
    [SerializeField] private float _huntAcceleration;

    [Header("Visual")] [SerializeField] private Material _material;
    
    private DialogSystem _dialogSystem => FindFirstObjectByType<DialogSystem>();
    private PlayerInput _input => FindFirstObjectByType<PlayerInput>();
    private RandomEvents _events => FindFirstObjectByType<RandomEvents>();
    private NavMeshAgent _agent => GetComponent<NavMeshAgent>();

    private RobotState _state = RobotState.Normal;
    private bool _isAgentStop, _isTargeting;
    private float _defaultSpeed, _defaultAcceleration;
    private Color _statColor;

    private void Start()
    {
        StartCoroutine(WalkRoutine());
        
        _defaultSpeed = _agent.speed;
        _defaultAcceleration = _agent.acceleration;
        _statColor = _material.GetColor(FresnelColor);
        
        PlayerMovement.OnRun += () =>
        {
            _isTargeting = true;
            _state = RobotState.Hunt;
            _agent.speed = _huntSpeed;
            _agent.acceleration = _huntAcceleration;
        };
        PlayerMovement.OnRunEnd += () =>
        {
            _isTargeting = false;
        };
    }

    private void Update()
    {
        if (!_agent.hasPath)
        {
            _isAgentStop = true;
            if (_state == RobotState.Hunt)
            {
                _state = RobotState.Normal;
                _agent.speed = _defaultSpeed;
                _agent.acceleration = _defaultAcceleration;
                StartCoroutine(WalkRoutine());
            }
        }

        if (_isTargeting)
        {
            _agent.SetDestination(_player.position);
        }
    }

    private IEnumerator WalkRoutine()
    {
        var currentPos = 0;
        var forward = true;
        StartCoroutine(ColorRoutine());
        while (_state == RobotState.Normal)
        {
            _agent.SetDestination(_positions[currentPos].position);
            forward = forward == Random.Range(0, 100) < 90;
            if (forward && currentPos == _positions.Count - 1)
                currentPos = -1;
            else if(!forward && currentPos == 0)
                currentPos = _positions.Count;
            currentPos = forward? currentPos + 1 : currentPos - 1;
            
            yield return new WaitUntil(() => _isAgentStop);
            _isAgentStop = false;
        }
    }

    private IEnumerator ColorRoutine()
    {
        var intensity = 0f;
        while (_state == RobotState.Normal)
        {
            yield return new WaitForSeconds(Random.Range(2,5));
            var targetIntensivity =  Random.Range(0, 2) == 1? Mathf.Pow(2,1.4f):0;
            while (!Mathf.Approximately(intensity, targetIntensivity))
            {
                intensity = Mathf.Lerp(intensity, targetIntensivity, Time.fixedDeltaTime);
                _material.SetColor(FresnelColor,
                    new Color(_statColor.r * intensity, _statColor.g * intensity, _statColor.b * intensity));
                yield return new WaitForFixedUpdate();
            }
            
        }
    }
    
    // public async void OnTriggerEnter(Collider sbj)
    // {
    //     if (sbj.CompareTag("Player"))
    //     {
    //         GetComponent<Collider>().enabled = false;
    //         sbj.transform.LookAt(transform);
    //         _input.enabled = false;
    //         
    //         var fragment = new DialogFragment
    //             { Text = RandomParamSt.RobotsReplics[Random.Range(0,RandomParamSt.RobotsReplics.Count)], Buttons = new()};
    //         _dialogSystem.FragmentsStack = new() { fragment };
    //         _dialogSystem.PlayNext();
    //         
    //         await Task.Delay(3000);
    //         _dialogSystem.EndChat();
    //         _events.Lose();
    //
    //         _input.enabled = true;
    //     }
    // }
}

public enum RobotState
{
    Normal,
    Hunt,
    Attack
}