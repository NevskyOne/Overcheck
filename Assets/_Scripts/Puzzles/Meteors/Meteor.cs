using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float _speed;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
    }

    public void OnClick()
    {
        Destroy(gameObject);
    }
}