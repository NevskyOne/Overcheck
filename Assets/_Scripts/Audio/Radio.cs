using System;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, IInteractable, IUsable
{
    [SerializeField] private List<AudioClip> _clips;
    [SerializeField] private AudioSource _source;
    [SerializeField] private int _strangeWave;
    public Material RadioMat;
        
    private int _currentWave;
    public int CursorInd { get; set; } = 4;

    public static event Action OnStrangeWave;

    public void Use()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _currentWave = _currentWave == (_clips.Count - 1)? 0 : _currentWave + 1;
        _source.resource = _clips[_currentWave];
        if(_currentWave == _strangeWave)
            OnStrangeWave?.Invoke();
        _source.Play();
        transform.GetChild(0).gameObject.SetActive(true);
    }
    public void Interact()
    {
        _source.mute = !_source.mute;
    }

    public void Uninteract() { }
}
