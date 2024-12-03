using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class NPCAnim : MonoBehaviour
{
    public bool IsTalking { private get; set; }
    
    private static readonly int _forward = Animator.StringToHash("Forward");
    private static readonly int _left = Animator.StringToHash("Left");
    private static readonly int _right = Animator.StringToHash("Right");
    private static readonly int _say = Animator.StringToHash("Say");
    private NavMeshAgent _agent => GetComponent<NavMeshAgent>();
    private Animator _animator => transform.GetChild(0).GetChild(0).GetComponent<Animator>();

    private void Update()
    {
        if (_agent.velocity.normalized.magnitude > 0.1f)
        {
            Vector3 direction = _agent.velocity.normalized;
            _animator.SetFloat(_forward, direction.magnitude);
        }
        else
        {
            _animator.SetBool(_say, IsTalking);
        }
    }

    public void TurnLeft()
    {
        _animator.SetTrigger(_left);
        _animator.SetFloat(_forward, 0);
    }

    public void TurnRight()
    {
        _animator.SetTrigger(_right);
        _animator.SetFloat(_forward, 0);
    }
}
