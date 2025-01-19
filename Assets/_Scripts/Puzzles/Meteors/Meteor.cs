using System;
using UnityEngine;
using UnityEngine.UI;

public class Meteor : MonoBehaviour
{
    public event Action OnDestroyMeteor;
    public event Action<GameObject> OnKickStation;
    
    private Transform _stationTransform;
    private float _speed;

    private void Update()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (transform.localPosition.x <= _stationTransform.localPosition.x)
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
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Image>().enabled = false;
        OnDestroyMeteor?.Invoke();
        this.enabled = false;
        Destroy(gameObject, 2f);
    }
}