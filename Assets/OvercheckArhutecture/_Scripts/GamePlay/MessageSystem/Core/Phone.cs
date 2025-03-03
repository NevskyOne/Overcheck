using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Phone : MonoBehaviour
{
    [SerializeField] private AudioSource _callSource;
    [SerializeField] private AudioSource _talkSource;
    [SerializeField] private TMP_Text _subtitleText;
    
    private CallMessage _currentCall;
    private bool _isCalling;
    private bool _isTalking;
    
    public void CallMessage(CallMessage msg)
    {
        if (!_isTalking && !_isCalling)
            StartCoroutine(CallRoutine(msg));
    }

    public void AnswerCall()
    {
        if (!_isCalling) return;
        
        _isTalking = true;
        _isCalling = false;
        _callSource.Stop();
        var call = _currentCall.CallSound;
        _talkSource.clip = call;
        _talkSource.Play();
        StartCoroutine(SubtitleRoutine(_currentCall.Subtitles));
    }

    private IEnumerator SubtitleRoutine(List<SubtitleEntry> subtitles)
    {
        while (_callSource.isPlaying)
        {
            var currentTime = _callSource.time;
            var currentSubtitle = "";

            foreach (var subtitle in subtitles)
            {
                if (currentTime >= subtitle.StartTime && currentTime <= subtitle.EndTime)
                {
                    currentSubtitle = subtitle.Text;
                    break;
                }
            }
            
            _subtitleText.text = currentSubtitle;
            yield return null;
        }
    }
    
    private IEnumerator CallRoutine(CallMessage msg)
    {
        _currentCall = msg;
        _callSource.Play();
        _isCalling = true;
        yield return new WaitForSeconds(msg.CallTime);
        _isCalling = false;
        _callSource.Stop();
        _currentCall = null;
    } 
}