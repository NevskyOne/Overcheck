using System;
using UnityEngine;
using Zenject;

public class Saturation:MonoBehaviour
{
    private static uint _saturation = 100;
    private static VisualEffects _fx;
    private static MainUI _mainUI;
    private static Player _player;
    
    [Inject]
    private void Initialize(VisualEffects fx, EventBus eventBus, MainUI mainUI, Player player)
    {
        _fx = fx;
        _player = player;
        _mainUI = mainUI;
        eventBus.Subscribe((NPCControledEvent _) => { ChangeSaturation(-15); });
        print("init");
    }
    
    public static void ChangeSaturation(int value)
    {
        print(value);
        _saturation = (uint)Math.Clamp(_saturation + value,0,100);
        
        if (value < 0) 
            _mainUI.DrainSaturation(_saturation); 
        else 
            _mainUI.FillSaturation(_saturation);

        if (_saturation <= 0)
        {
            _player.Die();
        }
        else if (_saturation < 20)
        {
            _fx.Starve(1, 0.3f, 0.5f, 0.5f);
            _mainUI.ChangeSaturColor(Color.red);
        }
        else
        {
            _fx.Starve(0, 0, 0, 0);
            _mainUI.ChangeSaturColor(Color.white);
        }
    }
    
}
