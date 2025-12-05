using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public static InventoryItem instance;

    [Header("Slots")]
    public Image[] slotImages;     // Image_item(1→10)
    public TMP_Text[] slotTexts;   // Text trong từng ô

    [Header("Colors")]
    public Color selectedColor = new Color(0.984f, 1f, 0.423f, 1f); // màu highlight
    public Color slotEmptyColor = new Color(1f, 1f, 1f, 0f);        // alpha = 0 → trong suốt

    private void Awake()
    {
        instance = this;

        // Khi game start, set tất cả slot trống → alpha = 0
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].color = slotEmptyColor;
            slotTexts[i].text = "";
        }
    }

    // Thêm item vào slot trống
    public void AddItem(Sprite icon, string name)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i].sprite == null)
            {
                slotImages[i].sprite = icon;
                slotTexts[i].text = name;

                // Khi có item → alpha = 1
                Color c = slotImages[i].color;
                c.a = 1f;
                slotImages[i].color = c;
                return;
            }
        }
    }

    // Kiểm tra slot có item không
    public bool IsSlotOccupied(int index)
    {
        return slotImages[index].sprite != null;
    }

    // Lấy tên item trong slot
    public string GetItemName(int index)
    {
        if (index >= 0 && index < slotTexts.Length)
            return slotTexts[index].text;
        return null;
    }

    // Set màu highlight khi chọn
    public void SetSlotColor(int index, Color color)
    {
        if (index >= 0 && index < slotImages.Length)
            slotImages[index].color = color;
    }

    // Xóa item trong slot → image trong suốt
    public void ClearButtonItem(int index)
    {
        if (index < 0 || index >= slotImages.Length) return;

        slotImages[index].sprite = null;
        slotTexts[index].text = "";

        // alpha = 0 → trong suốt
        Color c = slotImages[index].color;
        c.a = 0f;
        slotImages[index].color = c;
    }
}
