using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySlotSelector : MonoBehaviour
{
    public Color selectedColor = new Color(0.984f, 1f, 0.423f, 1f); // vàng sáng
    private Color originalColor;
    private List<Image> selectedSlots = new List<Image>(); // lưu các ô đã chọn (tối đa 2)

    void Start()
    {
        // Lấy màu gốc từ Image đầu tiên (màu mờ ban đầu)
        originalColor = GetComponentsInChildren<Image>()[0].color;

        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => OnSlotClick(btn));
        }
    }

    void OnSlotClick(Button btn)
    {
        Image img = btn.GetComponent<Image>();

        // Nếu ô này đã được chọn → bỏ chọn
        if (selectedSlots.Contains(img))
        {
            img.color = originalColor;
            selectedSlots.Remove(img);
            return;
        }

        // Nếu đang có 2 ô rồi → tắt 2 ô cũ
        if (selectedSlots.Count >= 2)
        {
            foreach (var slot in selectedSlots)
            {
                slot.color = originalColor;
            }
            selectedSlots.Clear();
        }

        // Chọn ô mới
        img.color = selectedColor;
        selectedSlots.Add(img);
    }
}
