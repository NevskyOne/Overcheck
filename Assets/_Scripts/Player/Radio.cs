using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _clips;
    [SerializeField] private AudioSource _source;
        
    private int _currentWave;
    
    public void ChangeClip()
    {
        _currentWave = _currentWave == (_clips.Count - 1)? 0 : _currentWave + 1;
        _source.resource = _clips[_currentWave];
        _source.Play();
    }
}
