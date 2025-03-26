using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectSystem : MonoBehaviour
{
    [SerializeField] private Transform _iconsHolder;
    [SerializeField] private Transform _menuHolder;
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private GameObject _menuPrefab;
    
    private readonly List<FoodEffect> _currentEffects = new();

    public void AddEffect(FoodEffect effect)
    {
        if(_currentEffects.Contains(effect)) return;
        
        _currentEffects.Add(effect);
        effect.ApplyEffect();
        
        var icon = Instantiate(_iconPrefab, _iconsHolder).transform;
        icon.GetChild(0).GetComponent<Image>().sprite = effect.Icon;
        
        var menu = Instantiate(_menuPrefab, _menuHolder).transform;
        menu.GetChild(0).GetComponent<TMP_Text>().text = effect.Description;
        menu.GetChild(1).GetComponent<Image>().sprite = effect.Icon;
    }
    
    public void RemoveEffect(FoodEffect effect)
    {
        if(!_currentEffects.Contains(effect)) return;

        int index = _currentEffects.IndexOf(effect);
        Destroy(_iconsHolder.GetChild(index));
        Destroy(_menuHolder.GetChild(index));
        
        _currentEffects.Remove(effect);
        effect.RemoveEffect();
    }
    
    public void RemoveEffects()
    {
        foreach (var effect in _currentEffects)
        {
            RemoveEffect(effect);
        }
    }
}
