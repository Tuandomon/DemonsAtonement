using UnityEngine;

public class ChestOpenOnce : MonoBehaviour
{
    [Header("References")]
    public GameObject buttonUI;
    public Transform player;
    public Animator chestAnimator;

    [Header("Settings")]
    public string openAnimationName = "Chest_wood"; // tên animation mở rương

    private bool isPlayerNear = false;
    private bool isOpened = false;

    void Start()
    {
        // Ẩn nút E ban đầu và tắt Animator để rương không tự mở
        if (buttonUI != null)
            buttonUI.SetActive(false);

        if (chestAnimator != null)
            chestAnimator.enabled = false;
    }

    void Update()
    {
        // Nếu player gần, rương chưa mở và nhấn E => mở rương
        if (isPlayerNear && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            if (chestAnimator != null)
            {
                chestAnimator.enabled = true;
                chestAnimator.Play(openAnimationName, 0, 0f); // chạy từ frame đầu
            }

            isOpened = true;

            // ✅ Xóa nút E hoàn toàn để nó không hiện lại
            if (buttonUI != null)
                Destroy(buttonUI);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            isPlayerNear = true;
            if (buttonUI != null)
                buttonUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;

            // Nếu rương chưa mở thì ẩn nút, nếu đã mở thì thôi
            if (!isOpened && buttonUI != null)
                buttonUI.SetActive(false);
        }
    }
}
