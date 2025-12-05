using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CombinationManager : MonoBehaviour
{
    public GameObject combinationPanel; // Panel_item combination(1)
    private List<int> selectedIndices = new List<int>(); // lưu index của 2 ô được chọn

    public InventoryItem inventory; // tham chiếu đến InventoryItem

    private void Start()
    {
        combinationPanel.SetActive(false);
    }

    // Gọi khi nhấn Button_item combination
    public void ToggleCombinationPanel()
    {
        combinationPanel.SetActive(!combinationPanel.activeSelf);
        selectedIndices.Clear();
    }

    // Gọi khi nhấn vào 1 ô item trong panel combination
    public void OnItemClicked(int slotIndex)
    {
        if (!inventory.IsSlotOccupied(slotIndex)) return;

        // Bỏ chọn nếu nhấn lại
        if (selectedIndices.Contains(slotIndex))
        {
            selectedIndices.Remove(slotIndex);
            inventory.SetSlotColor(slotIndex, inventory.slotEmptyColor);
            return;
        }

        // Nếu đã chọn 2 ô rồi → reset
        if (selectedIndices.Count >= 2)
        {
            foreach (var i in selectedIndices)
                inventory.SetSlotColor(i, inventory.slotEmptyColor);
            selectedIndices.Clear();
        }

        // Chọn ô mới
        selectedIndices.Add(slotIndex);
        inventory.SetSlotColor(slotIndex, inventory.selectedColor);

        // Nếu đã chọn đủ 2 item → ghép
        if (selectedIndices.Count == 2)
        {
            int indexA = selectedIndices[0];
            int indexB = selectedIndices[1];

            string nameA = inventory.GetItemName(indexA);
            string nameB = inventory.GetItemName(indexB);

            if (CanCombine(nameA, nameB))
            {
                // Xóa hình và text trực tiếp trên Button_item
                inventory.ClearButtonItem(indexA);
                inventory.ClearButtonItem(indexB);
            }

            // Reset màu highlight → slot trống = trong suốt
            inventory.SetSlotColor(indexA, inventory.slotEmptyColor);
            inventory.SetSlotColor(indexB, inventory.slotEmptyColor);
            selectedIndices.Clear();
        }
    }

    // Kiểm tra 2 item có ghép được không
    bool CanCombine(string a, string b)
    {
        return (a == "Mảnh chữ gợi" && b == "Mảnh chữ Y") ||
               (a == "Mảnh chữ Y" && b == "Mảnh chữ gợi");
    }
}
