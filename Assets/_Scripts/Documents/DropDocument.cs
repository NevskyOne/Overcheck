using UnityEngine;

public class DropDocument : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("BackDoc")) return;
        NPCManager.CurrentNPC.CollectDoc(gameObject);
    }
    public void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("BackDoc")) return;
        NPCManager.CurrentNPC.CollectDoc(gameObject, false);
    }
}
