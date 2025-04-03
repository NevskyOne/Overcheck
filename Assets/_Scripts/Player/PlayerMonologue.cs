using Random = System.Random;
using System.Collections.Generic;

public class PlayerMonologue
{
    private MonologueStruct _struct;
    private DialogSystem _dialogSystem;

    public PlayerMonologue(DialogSystem sys, MonologueStruct monologue)
    {
        _dialogSystem = sys;
        _struct = monologue;
        // TimeLines.OnDayEnd += () => PlayMonologue(_dayStartPhrases);
        // NPCManager.OnNPCEnd += () => PlayMonologue(_dayEndPhrases);
        // NPCManager.RandomEvent += () => PlayMonologue(_eventStartPhrases);
        // RandomEvents.OnDone += () => PlayMonologue(_eventDonePhrases);
        // Radio.OnStrangeWave += () => PlayMonologue(_strangeVoicePhrases);
        // NPCManager.EternityCheck += () => PlayMonologue(_eternityPhrases);
    }
    
    private Random _rnd = new Random();

    private void PlayMonologue(List<string> list)
    {
        _dialogSystem.FragmentsStack = new(){
            new DialogFragment { Text = list[_rnd.Next(list.Count)], Buttons = new ButtonSt[0]}};
        _dialogSystem.PlayNext();
        Player.State = PlayerState.Dialog;
    }
}
