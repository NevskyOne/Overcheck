using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class Switcher : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private List<string> _values;
        private int _value;

        public void UpdateUI(int value)
        {
            _value = value;
            _text.text = _values[_value];
        }

        public void ChangeGraphics(int delta)
        {
            _value += delta;
            if (_value > _values.Count - 1) 
                _value = 0;
            else if(_value < 0)
                _value = _values.Count - 1;
            UpdateUI(_value);
            SettingsUI.Graphics = _value;
        }
        
        public void ChangeVFX(int delta)
        {
            _value += delta;
            if (_value > _values.Count - 1) 
                _value = 0;
            else if(_value < 0)
                _value = _values.Count - 1;
            UpdateUI(_value);
            SettingsUI.ChangeVFX(_value == 1);
        }
    }
}