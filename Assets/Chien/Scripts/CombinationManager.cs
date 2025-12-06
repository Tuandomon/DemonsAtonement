using UnityEngine;
using System.Collections.Generic;

public class CombinationManager : MonoBehaviour
{
    public GameObject combinationPanel; // Panel_item combination(1)
    public InventoryItem inventory; // tham chiếu đến InventoryItem

    public Sprite suggestionSprite;  // sprite cho item Gợi ý
    public string suggestionName = "Gợi ý"; // tên item Gợi ý

    private List<int> selectedIndices = new List<int>(); // lưu index của 2 ô được chọn

    private void Start()
    {
        if (combinationPanel != null)
            combinationPanel.SetActive(false);
    }

    public void ToggleCombinationPanel()
    {
        if (combinationPanel != null)
            combinationPanel.SetActive(!combinationPanel.activeSelf);
        selectedIndices.Clear();
    }

    public void OnItemClicked(int slotIndex)
    {
        // Chỉ cho phép ghép khi panel combination đang hiển thị
        if (!combinationPanel.activeSelf) return;
        if (!inventory.IsSlotOccupied(slotIndex)) return;

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
                // Xóa item cũ
                inventory.ClearButtonItem(indexA);
                inventory.ClearButtonItem(indexB);

                // Thêm item Gợi ý vào ô indexA
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
