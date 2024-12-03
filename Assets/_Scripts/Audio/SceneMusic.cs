
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private List<AudioClip> _calmMusic;
    [SerializeField] private AudioClip _wiresMusic, _gearMusic, _meteoriteMusic;
    
    
    private MusicState _state = MusicState.Wires;
    public static MusicState State { get; set; } = MusicState.Normal;
    
    private Random _rnd = new Random();

    private void Update()
    {
        if (State != _state)
        {
            _state = State;
            switch (_state)
            {
                case MusicState.Normal:
                    _source.clip = _calmMusic[_rnd.Next(3)];
                    break;
                case MusicState.Wires:
                    _source.clip = _wiresMusic;
                    break;
                case MusicState.Gears:
                    _source.clip = _gearMusic;
                    break;
                case MusicState.Meteorites:
                    _source.clip = _meteoriteMusic;
                    break;
            }
            _source.Play();
        }
    }
}

public enum MusicState
{
    Normal, Wires, Gears, Meteorites
}