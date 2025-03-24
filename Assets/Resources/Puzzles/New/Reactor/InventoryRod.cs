using UnityEngine;

public class InventoryRod : MonoBehaviour
{
    public ReactorRod rodPrefab;
    private ReactorRod currentRod;

    private void Start() => SpawnRod();

    public void SpawnRod()
    {
        if (currentRod != null) return;

        currentRod = Instantiate(rodPrefab, transform);
        currentRod.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public ReactorRod TakeRod()
    {
        if (currentRod == null) return null;

        var rod = currentRod;
        currentRod = null;
        Invoke(nameof(SpawnRod), 0.5f);
        return rod;
    }
}