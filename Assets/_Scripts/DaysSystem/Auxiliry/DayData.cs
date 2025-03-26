using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DayData
{
    [SerializeField] private List<StoryData> _stories = new();
    [SerializeField] private List<ConditionalEvent> _conditionalEvents = new();
    [SerializeField] private int _randomNPCCount;

    public List<string> InvokedEvents;
    public List<NPCData> NPCs { get; private set; } = new();
    
    public List<StoryData> Stories => _stories;
    public List<ConditionalEvent> ConditionalEvents => _conditionalEvents;
    public int RandomNpcCount => _randomNPCCount;
}