using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerHider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
