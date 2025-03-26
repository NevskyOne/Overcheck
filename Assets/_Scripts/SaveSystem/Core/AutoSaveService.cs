using System.Collections;
using UnityEngine;
using Zenject;

public class AutoSaveService : MonoBehaviour
{
    [SerializeField] private float _autoSaveInterval;
    
    private EventBus _eventBus;
    
    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _eventBus = eventBus;

        StartCoroutine(AutoSaveRoutine());
    }
    
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_autoSaveInterval);
            _eventBus.Invoke(new AutoSaveEvent());
        }
    }
}