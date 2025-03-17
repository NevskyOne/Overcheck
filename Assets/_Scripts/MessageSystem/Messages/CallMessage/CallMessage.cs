using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CallMessage", menuName = "Messages/CallMessage")]
public class CallMessage : Message
{
    [SerializeField] private AudioClip _callSound;
    [SerializeField] private List<SubtitleEntry> _subtitles = new();
    [SerializeField] private float _callTime;

    public AudioClip CallSound => _callSound;

    public List<SubtitleEntry> Subtitles => _subtitles;
    
    public float CallTime => _callTime;
}