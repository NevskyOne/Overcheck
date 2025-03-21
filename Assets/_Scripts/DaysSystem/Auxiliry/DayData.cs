using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct DayData
{
    [SerializeField] private List<Message> _messages;
    [SerializeField] private List<ConditionalEvent> _conditionalEvents;
    [SerializeField] private int _randomNPCCount;

    public List<string> InvokedEvents;
    public List<NPCData> NPCs { get; private set; }
    
    public List<Message> Messages => _messages;
    public List<ConditionalEvent> ConditionalEvents => _conditionalEvents;
    public int RandomNpcCount => _randomNPCCount;
}