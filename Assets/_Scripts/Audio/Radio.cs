using System;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _clips;
    [SerializeField] private AudioSource _source;
    [SerializeField] private int _strangeWave;
    public Material RadioMat;
        
    private int _currentWave;

    public static event Action OnStrangeWave;
    
    public void ChangeClip()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _currentWave = _currentWave == (_clips.Count - 1)? 0 : _currentWave + 1;
        _source.resource = _clips[_currentWave];
        if(_currentWave == _strangeWave)
            OnStrangeWave?.Invoke();
        _source.Play();
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
