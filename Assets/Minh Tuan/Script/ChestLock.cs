using UnityEngine;

public class ChestLock : MonoBehaviour
{
    [Header("Chest Prefab mở")]
    [SerializeField] private GameObject openChestPrefab;

    [Header("Panel Lock")]
    [SerializeField] private GameObject panelObj;

    private LockPanelT9 lockPanelScript;

    private bool isPanelOpen = false;
    private bool playerInside = false;   // 🔥 kiểm tra player đứng trong vùng

    private void Start()
    {
        if (panelObj != null)
        {
            lockPanelScript = panelObj.GetComponent<LockPanelT9>();
            if (lockPanelScript != null)
            {
                lockPanelScript.onUnlock.AddListener(UnlockChest);
            }

            panelObj.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel Lock chưa được gán trong Inspector!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // Nếu rời vùng → tự tắt panel
            if (panelObj != null && isPanelOpen)
            {
                isPanelOpen = false;
                panelObj.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // 🔥 Bấm E để toggle panel
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            isPanelOpen = !isPanelOpen;
            panelObj.SetActive(isPanelOpen);
        }
    }

    private void UnlockChest()
    {
        panelObj.SetActive(false);

        if (openChestPrefab)
        {
            Instantiate(openChestPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
