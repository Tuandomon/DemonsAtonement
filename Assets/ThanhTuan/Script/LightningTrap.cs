using UnityEngine;
using System.Collections;

public class LightningTrap : MonoBehaviour
{
    // --- CÀI ĐẶT CHU KỲ & ANIMATION ---
    private Animator lightningAnimator;

    [Header("Chu kỳ & Animation")]
    public float safeDuration = 2.5f;   // Thời gian Cooldown (An toàn)
    public float activeDuration = 1.5f; // Thời gian giật sét (Nguy hiểm)
    private bool isShocking = false;    // Kiểm soát bẫy có đang gây sát thương không

    // Tên Trigger cho Animator
    private readonly string ACTIVE_TRIGGER = "StartShock";

    // --- CÀI ĐẶT SÁT THƯƠNG ---
    [Header("Cài đặt Sát thương")]
    // Đổi tên biến để phản ánh sát thương mỗi lần gọi hàm (mỗi frame)
    public int damagePerHit = 5;
    public string targetTag = "Player";


    void Awake()
    {
        lightningAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        isShocking = false;
        StartCoroutine(ShockCycle());
    }

    IEnumerator ShockCycle()
    {
        while (true)
        {
            // GIAI ĐOẠN COOLDOWN
            isShocking = false;
            yield return new WaitForSeconds(safeDuration);

            // GIAI ĐOẠN NGUY HIỂM
            isShocking = true;
            if (lightningAnimator != null)
            {
                lightningAnimator.SetTrigger(ACTIVE_TRIGGER); // Kích hoạt animation sét giật
            }

            yield return new WaitForSeconds(activeDuration);
        }
    }

    // --- LOGIC GÂY SÁT THƯƠNG LIÊN TỤC (PER FRAME) ---
    // Được gọi liên tục chừng nào Collider còn chồng lấn
    private void OnTriggerStay2D(Collider2D other)
    {
        // Chỉ gây sát thương khi bẫy đang ở trạng thái nguy hiểm (isShocking = true)
        if (other.CompareTag(targetTag) && isShocking)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Gây sát thương ngay lập tức, giống như FireTrap
                playerHealth.TakeDamage(damagePerHit);
                // Lưu ý: Sát thương này xảy ra mỗi frame (khoảng 60 lần/giây)
                // Bạn nên giảm damagePerHit xuống mức rất thấp (ví dụ: 5 hoặc 10)
            }
        }

        // --- ĐÃ LOẠI BỎ LOGIC nextDamageTime và OnTriggerEnter2D ---
    }
}