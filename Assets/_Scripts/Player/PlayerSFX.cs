using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private AudioSource _breathSource, _feetSource;
    
    public void PlayBreath(bool flag = true)
    { 
        if(flag)
            _breathSource.Play();
        else
            _breathSource.Stop();
    }
    
    public void PlayFeet(bool flag = true)
    { 
        if(flag && !_feetSource.isPlaying)
            _feetSource.Play();
        else if(_feetSource.isPlaying)
            _feetSource.Stop();
    }
}
