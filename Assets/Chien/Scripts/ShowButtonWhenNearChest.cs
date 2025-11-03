using UnityEngine;

public class ShowButtonWhenNearChest : MonoBehaviour
{
    public GameObject buttonUI;       // Nút E hiển thị khi player lại gần
    public Transform player;          // Tham chiếu đến nhân vật player
    public ChestOpenOnce chestScript; // Tham chiếu đến script ChestOpenOnce của rương

    private bool isPlayerNear = false;

    void Start()
    {
        if (buttonUI != null)
            buttonUI.SetActive(false);
    }

    void Update()
    {
        // Nếu rương đã mở rồi => ẩn nút E và dừng luôn
        if (chestScript != null && chestScript.IsOpened())
        {
            if (buttonUI != null)
                buttonUI.SetActive(false);
            enabled = false; // Tắt script này luôn để ngăn việc hiển thị lại
            return;
        }

        // Nếu player ở gần thì hiện nút E
        if (isPlayerNear && buttonUI != null)
        {
            buttonUI.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (buttonUI != null)
                buttonUI.SetActive(false);
        }
    }
}
