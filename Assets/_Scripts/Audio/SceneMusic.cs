
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private List<AudioClip> _calmMusic;
    [SerializeField] private AudioClip _wiresMusic, _gearMusic, _meteoriteMusic;
    
    
    private MusicState _state;
    public static MusicState State { get; set; }
    
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
        }
    }
}

public enum MusicState
{
    Normal, Wires, Gears, Meteorites
}