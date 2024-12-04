using System;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public event Action OnDestroyMeteor;
    public event Action<GameObject> OnKickStation;
    
    private Transform _stationTransform;
    private float _speed;

    private void Update()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        if (transform.position.x <= _stationTransform.position.x)
        {
            OnKickStation?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }
    
    public void SetProperties(Transform station, float speed)
    {
        _stationTransform = station;
        _speed = speed;
    }
    
    public void OnClick()
    {
        OnDestroyMeteor?.Invoke();
        Destroy(gameObject);
    }
}