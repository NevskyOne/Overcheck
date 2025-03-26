using System;
using UnityEngine;
using Zenject;

public class Saturation:MonoBehaviour
{
    private static uint _saturation = 100;
    private static VisualEffects _fx;
    private static MainUI _mainUI;
    
    [Inject]
    private void Initialize(VisualEffects fx, EventBus eventBus, MainUI mainUI)
    {
        _fx = fx;
        _mainUI = mainUI;
        eventBus.Subscribe((NPCControledEvent _) => { ChangeSaturation(-20); });
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

        if (_saturation < 30)
        {
            _fx.Starve(1, 0.3f, 0.5f, 0.5f);
            _mainUI.ChangeSaturColor(Color.red);
        }
        else
        {
            _fx.Starve(0.3f, 0, 0, 0);
            _mainUI.ChangeSaturColor(Color.white);
        }
    }
    
}
