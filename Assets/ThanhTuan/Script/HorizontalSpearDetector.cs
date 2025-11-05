using UnityEngine;

// Script này gắn vào Collider là con của Bẫy (Activation Zone)
public class HorizontalSpearDetector : MonoBehaviour
{
    private HorizontalSpearLauncher trapLauncher; // Tham chiếu đến script bẫy mới

    void Start()
    {
        // Lấy script HorizontalSpearLauncher từ đối tượng cha (nơi script bẫy được gắn)
        trapLauncher = GetComponentInParent<HorizontalSpearLauncher>();

        if (trapLauncher == null)
        {
            Debug.LogError("HorizontalSpearDetector: Phải là con của GameObject có script HorizontalSpearLauncher!");
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra Tag của vật thể va chạm có phải là mục tiêu không
        // 2. Kiểm tra bẫy có đang hồi chiêu (cooldown) không
        if (other.CompareTag(trapLauncher.targetTag) && !trapLauncher.isOnCooldown)
        {
            // Gọi hàm ActivateTrap() để kích hoạt cơ chế phóng và hồi chiêu
            // Chúng ta gọi hàm public ActivateTrap() thay vì Coroutine trực tiếp
            trapLauncher.ActivateTrap();
        }
    }
}