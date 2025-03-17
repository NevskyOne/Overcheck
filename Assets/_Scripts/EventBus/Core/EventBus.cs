using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    private Dictionary<string, List<object>> _events = new();
    
    public void Subscribe<T>(Action<T> callback)
    {
        var key = typeof(T).Name;

        if (_events.ContainsKey(key))
            _events[key].Add(callback);
        else
            _events.Add(key, new List<object> { callback });
    }

    public void Unsubscribe<T>(Action<T> callback)
    {
        var key = typeof(T).Name;
        
        if (_events.ContainsKey(key))
            _events[key].Remove(callback);
        else
            Debug.Log("Trying to unsubscribe not exist callback");
    }

    public void Invoke<T>(T eventToInvoke)
    {
        var key = typeof(T).Name;
        if (_events.ContainsKey(key))
        {
            foreach (var obj in _events[key])
            {
                var callback = obj as Action<T>;
                callback?.Invoke(eventToInvoke);
                Debug.Log($"Callback: {callback.Method.Name}, Event: {eventToInvoke}, Key: {key}");
            }
        }
    }
}