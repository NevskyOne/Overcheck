using UnityEngine;
using UnityEngine.EventSystems;

public class ReactorRodSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private ReactorQuiz quiz;
    [SerializeField] private RectTransform dropZone;
    public int slotIndex;
    private ReactorRod currentRod;

    private void Start()
    {
        if (dropZone == null)
            Debug.LogError("DropZone не назначена для слота " + slotIndex);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.TryGetComponent(out ReactorRod rod)) return;

        if (currentRod != null)
            quiz.ReturnRodToInventory(currentRod);

        rod.transform.position = GetSlotCenter();
        rod.SetSlot(this);
        quiz.OnRodPlaced(slotIndex, rod);
        currentRod = rod;
    }

    private Vector3 GetSlotCenter()
    {
        Vector3[] corners = new Vector3[4];
        dropZone.GetWorldCorners(corners);
        return (corners[0] + corners[2]) / 2;
    }

    public void ClearSlot()
    {
        if (currentRod != null)
            Destroy(currentRod.gameObject);
        currentRod = null;
    }
}