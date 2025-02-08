using System;

public static class Saturation
{
    private static uint _saturation = 100;
    //Добавить подписку на событие проверки доков
    public static void ChangeSaturation(uint value, bool add = true)
    {
        _saturation = Math.Clamp(add? _saturation + value: _saturation - value,0,100);
        if (_saturation < 20)
            Starve();
    }

    private static void Starve()
    {
        //TODO
    }
}
