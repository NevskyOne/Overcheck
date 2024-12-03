using UnityEngine;

public class DropDocument : MonoBehaviour
{
    public void OnCollisionEnter(Collision other)
    {
        if (!other.transform.CompareTag("BackDoc")) return;
        NPCManager.CurrentNPC.CollectDoc(gameObject);
    }
    public void OnCollisionExit(Collision other)
    {
        if (!other.transform.CompareTag("BackDoc")) return;
        NPCManager.CurrentNPC.CollectDoc(gameObject, false);
    }
}
