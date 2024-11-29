
using System;
using UnityEngine;
using UnityEngine.AI;

public class NPCAnim : MonoBehaviour
{
    private static readonly int Forward = Animator.StringToHash("Forward");
    private static readonly int Turn = Animator.StringToHash("Turn");
    private NavMeshAgent _agent => GetComponent<NavMeshAgent>();
    private Animator _animator => transform.GetChild(0).GetComponent<Animator>();

    private void Update()
    {
        Vector3 direction = _agent.velocity.normalized;
        _animator.SetFloat(Forward, direction.magnitude);
        // float angle = Vector3.SignedAngle(transform.forward, direction.normalized, Vector3.up);
        //
        // if (Mathf.Abs(angle) > 0.1f)
        // {
        //     _animator.SetFloat(Turn, angle);
        // }
        // else
        // {
        //     _animator.SetFloat(Turn, 0f);
        // }
    }
}
