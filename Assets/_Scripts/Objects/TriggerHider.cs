using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerHider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.gameObject.SetActive(false);
    }
}
