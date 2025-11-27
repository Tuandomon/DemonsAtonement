using UnityEngine;
using UnityEngine.UI;

public class CombinationUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelInventory;            // Panel Inventory
    public GameObject buttonItemCombination;     // Button mở/tắt combination
    public GameObject panelItemCombination;      // Panel combination

    private bool isCombinationOpen = false;

    void Start()
    {
        // Gắn sự kiện click vào button
        Button btn = buttonItemCombination.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(ToggleCombination);
        }
    }

    void Update()
    {
        // Nhấn phím N
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleCombination();
        }

        // Nếu Panel Inventory tắt → ẩn luôn Panel Combination
        if (!panelInventory.activeSelf && isCombinationOpen)
        {
            CloseCombination();
        }
    }

    public void ToggleCombination()
    {
        isCombinationOpen = !isCombinationOpen;
        panelItemCombination.SetActive(isCombinationOpen);
    }

    public void CloseCombination()
    {
        isCombinationOpen = false;
        panelItemCombination.SetActive(false);
    }
}
