using System;
using UnityEngine;

[Serializable]
public class SubtitleEntry
{
    [SerializeField] private float _startTime;
    [SerializeField] private float _endTime;
    [SerializeField] private string _text;

    public float StartTime => _startTime;

    public float EndTime => _endTime;

    public string Text => _text;
}