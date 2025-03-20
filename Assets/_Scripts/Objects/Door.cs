using UnityEngine;

public class Door : MonoBehaviour
{
    private static readonly int _open = Animator.StringToHash("Open");
    private Animator _animator => transform.GetComponentInChildren<Animator>();
    private AudioSource _source => GetComponent<AudioSource>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("HoloBear")) return;
        _animator.SetBool(_open, true);
        print("Open");
        _source.Play();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")&& !other.CompareTag("HoloBear")) return;
        _animator.SetBool(_open, false);
        print("Close");
        _source.Play();
    }
}
