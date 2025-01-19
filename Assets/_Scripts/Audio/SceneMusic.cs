
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private List<AudioClip> _calmMusic;
    [SerializeField] private AudioClip _wiresMusic, _gearMusic, _meteoriteMusic;

    private CrossfadeAudiosource _crossfade => GetComponent<CrossfadeAudiosource>();
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
                    _crossfade.Fade(_calmMusic[_rnd.Next(3)], 1);
                    _source.loop = false;
                    break;
                case MusicState.Wires:
                    _crossfade.Fade(_wiresMusic, 1);
                    _source.loop = true;
                    break;
                case MusicState.Gears:
                    _crossfade.Fade(_gearMusic, 1);
                    _source.loop = true;
                    break;
                case MusicState.Meteorites:
                    _crossfade.Fade(_meteoriteMusic, 1);
                    _source.loop = true;
                    break;
            }
        }
        else if (!_source.isPlaying && State == MusicState.Normal)
        {
            _source.clip = _calmMusic[_rnd.Next(3)];
            _source.Play();
        }
    }
}

public enum MusicState
{
    Normal, Wires, Gears, Meteorites
}