using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PMSDocument : Document
{
    
    public override void Randomize(int maxRandomCount)
    {
        base.Randomize(maxRandomCount);
        
        var randomCount = _rnd.Next(0, maxRandomCount);
        for (UInt16 i = 0; i < randomCount; i++)
        {
            var randomParam = _rnd.Next(0, _paramCount);
            switch (randomParam)
            {
                case 0:
                    _name = (RandomParamSt.Names.Except(new List<string>{_name})).ToList()
                        [_rnd.Next(0,RandomParamSt.Names.Count)];
                    break;
                case 1:
                    _photo = (RandomParamSt.Photos.Except(new List<Sprite>{_photo})).ToList()
                        [_rnd.Next(0,RandomParamSt.Photos.Count)];
                    break;
            }
        }
    }

    public void SetGoal(int value)
    {
        _npcManager.CurrentNPC.CurrentGoal = (Goal)value;
    }
}