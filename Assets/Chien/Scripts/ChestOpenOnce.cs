using UnityEngine;

public class ChestOpenOnce : MonoBehaviour
{
    public GameObject buttonUI;
    public Transform player;
    public Animator chestAnimator;

    private bool isPlayerNear = false;
    private bool isOpened = false;

    void Start()
    {
        buttonUI.SetActive(false);
        chestAnimator.enabled = false; // 🧩 Tắt animator để không tự chạy animation
    }

    void Update()
    {
        if (isPlayerNear && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            chestAnimator.enabled = true; // ✅ Bật lại Animator khi nhấn E
            chestAnimator.Play("Chest_wood"); // 🔁 Chạy animation "mở rương" (đặt đúng tên clip)
            isOpened = true;
            buttonUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            isPlayerNear = true;
            buttonUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            buttonUI.SetActive(false);
        }
    }

    public bool IsOpened()
    {
        return isOpened;
    }
}
