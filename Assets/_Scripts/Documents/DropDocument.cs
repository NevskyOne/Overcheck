using UnityEngine;

public class DropDocument : MonoBehaviour
{
    private NPCManager _npcMng;

    private void Start()
    {
        _npcMng = FindFirstObjectByType<NPCManager>();
    }

    public void OnCollisionEnter(Collision other)
    {
        if (!other.transform.CompareTag("BackDoc")) return;
        _npcMng.NPCCollectDoc();
        Destroy(gameObject);
    }
}