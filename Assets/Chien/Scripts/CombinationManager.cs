using UnityEngine;
using System.Collections.Generic;

public class CombinationManager : MonoBehaviour
{
    public GameObject combinationPanel;   // Panel_item combination(1)
    public GameObject panelChuGoiY;       // Panel chữ gợi ý
    public InventoryItem inventory;       // tham chiếu đến InventoryItem

    public Sprite suggestionSprite;       // sprite cho item Gợi ý
    public string suggestionName = "Gợi ý";

    private List<int> selectedIndices = new List<int>();

    // Double click support
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.25f;

    private void Start()
    {
        if (combinationPanel != null)
            combinationPanel.SetActive(false);

        if (panelChuGoiY != null)
            panelChuGoiY.SetActive(false);
    }

    public void ToggleCombinationPanel()
    {
        if (combinationPanel != null)
            combinationPanel.SetActive(!combinationPanel.activeSelf);

        selectedIndices.Clear();
    }

    public void OnItemClicked(int slotIndex)
    {
        // =========================
        // 🔥 1. KIỂM TRA DOUBLE-CLICK
        // =========================
        if (Time.time - lastClickTime <= doubleClickThreshold)
        {
            string itemName = inventory.GetItemName(slotIndex);

            if (itemName == suggestionName)  // nếu đúng là item Gợi ý
            {
                panelChuGoiY.SetActive(true); // mở panel chữ gợi ý
            }
        }

        lastClickTime = Time.time;


        // =========================
        // 🔥 2. TÍNH NĂNG GHÉP ITEM
        // =========================
        if (!combinationPanel.activeSelf)
            return;

        if (!inventory.IsSlotOccupied(slotIndex))
            return;

        if (!selectedIndices.Contains(slotIndex))
            selectedIndices.Add(slotIndex);

        if (selectedIndices.Count == 2)
        {
            int indexA = selectedIndices[0];
            int indexB = selectedIndices[1];

            string nameA = inventory.GetItemName(indexA);
            string nameB = inventory.GetItemName(indexB);

            if (CanCombine(nameA, nameB))
            {
                inventory.ClearButtonItem(indexA);
                inventory.ClearButtonItem(indexB);

                inventory.AddItemAtSlot(suggestionSprite, suggestionName, indexA);
            }

            selectedIndices.Clear();
        }
    }

    bool CanCombine(string a, string b)
    {
        return (a == "Mảnh chữ gợi" && b == "Mảnh chữ Y") ||
               (a == "Mảnh chữ Y" && b == "Mảnh chữ gợi");
    }
}
