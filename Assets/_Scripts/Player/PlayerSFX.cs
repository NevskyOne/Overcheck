using UnityEngine;

public class PlayerSFX
{
    private readonly AudioSource _breathSource;
    private readonly AudioSource _feetSource;

    public PlayerSFX(AudioSource breathSource, AudioSource feetSource)
    {
        _breathSource = breathSource;
        _feetSource = feetSource;
    }
    
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
        else if(!flag && _feetSource.isPlaying)
            _feetSource.Stop();
    }
}
