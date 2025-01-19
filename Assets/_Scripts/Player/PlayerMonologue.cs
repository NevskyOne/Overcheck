using Random = System.Random;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMonologue : MonoBehaviour
{
    [SerializeField] private List<string> _dayStartPhrases;
    [SerializeField] private List<string> _dayEndPhrases;
    [SerializeField] private List<string> _eventStartPhrases;
    [SerializeField] private List<string> _eventDonePhrases;
    [SerializeField] private List<string> _strangeVoicePhrases;
    [SerializeField] private List<string> _eternityPhrases;
    

    private DialogSystem _dialogSystem => FindFirstObjectByType<DialogSystem>();
    private Random _rnd = new Random();
    
    private void Start()
    {
        TimeLines.OnDayEnd += () => PlayMonologue(_dayStartPhrases);
        NPCManager.OnNPCEnd += () => PlayMonologue(_dayEndPhrases);
        NPCManager.RandomEvent += () => PlayMonologue(_eventStartPhrases);
        RandomEvents.OnDone += () => PlayMonologue(_eventDonePhrases);
        Radio.OnStrangeWave += () => PlayMonologue(_strangeVoicePhrases);
        NPCManager.EternityCheck += () => PlayMonologue(_eternityPhrases);
    }

    private void PlayMonologue(List<string> list)
    {
        _dialogSystem.FragmentsStack = new(){
            new DialogFragment { Text = list[_rnd.Next(list.Count)], Buttons = new()}};
        _dialogSystem.PlayNext();
        PlayerInteractions.PlayerState = PlayerState.Dialog;
    }
}
