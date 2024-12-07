using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class NPCAnim : MonoBehaviour
{
    public bool IsTalking { private get; set; }
    
    private static readonly int _forward = Animator.StringToHash("Forward");
    private static readonly int _say = Animator.StringToHash("Say");
    private NavMeshAgent _agent => GetComponent<NavMeshAgent>();
    private Animator _animator => transform.GetChild(0).GetChild(0).GetComponent<Animator>();

    private void Update()
    {
        if (_agent.velocity.magnitude > 0.3f)
        {
            Vector3 direction = _agent.velocity.normalized;
            _animator.SetFloat(_forward, direction.magnitude);
        }
        else
        {
            _animator.SetBool(_say, IsTalking);
            _animator.SetFloat(_forward, 0);
        }
    }

    public IEnumerator TurnLeft()
    {
        var targetRot = 90;
        
        while (!Mathf.Approximately(transform.localEulerAngles.y, targetRot))
        {
            transform.localEulerAngles = new Vector3(0,Mathf.LerpAngle(transform.localEulerAngles.y, targetRot, 0.1f),0);
            yield return null;
        }
        _animator.SetFloat(_forward, 0);
    }

    public IEnumerator TurnRight()
    {
        var targetRot = -90;
        
        while (!Mathf.Approximately(transform.localEulerAngles.y, targetRot))
        {
            transform.localEulerAngles = new Vector3(0,Mathf.LerpAngle(transform.localEulerAngles.y, targetRot, 0.1f),0);
            yield return null;
        }
        _animator.SetFloat(_forward, 0);
    }
}
