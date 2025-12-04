using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public static InventoryItem instance;

    public Image[] slotImages;     // Image_item(1) → Image_item(10)
    public TMP_Text[] slotTexts;   // Text trong từng ô

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(Sprite icon, string name)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i].sprite == null)
            {
                slotImages[i].sprite = icon;
                slotTexts[i].text = name;

                // 👉 Khi có item thì hiện rõ màu
                Color c = slotImages[i].color;
                c.a = 1f;                      // alpha = 1 (hiện rõ)
                slotImages[i].color = c;

                return;
            }
        }

        Debug.Log("Túi đầy rồi!");
    }
}
