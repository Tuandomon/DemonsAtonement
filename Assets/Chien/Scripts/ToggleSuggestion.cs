using UnityEngine;
using UnityEngine.UI;

public class ToggleSuggestion : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject panelInventory;          // Panel Inventory tổng
    public GameObject buttonSuggest;           // Nút Button_Suggest
    public GameObject panelSuggestionBoard;    // Panel_suggestion board

    private bool isOpen = false;

    void Start()
    {
        // Gán sự kiện click vào Button_Suggest
        Button btn = buttonSuggest.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(Toggle);
        }
    }

    void Update()
    {
        // Nếu Panel Inventory đang tắt → tắt Suggest Panel luôn
        if (!panelInventory.activeSelf && isOpen)
        {
            Close();
        }
    }

    void Toggle()
    {
        isOpen = !isOpen;
        panelSuggestionBoard.SetActive(isOpen);
    }

    void Close()
    {
        isOpen = false;
        panelSuggestionBoard.SetActive(false);
    }
}
