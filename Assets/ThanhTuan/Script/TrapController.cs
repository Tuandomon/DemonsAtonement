using UnityEngine;

public class TrapController : MonoBehaviour
{
    [Header("Visuals & Components")]
    // Kéo và thả hai Game Object con (Inactive/Active) vào đây trong Inspector
    [SerializeField] private GameObject inactiveVisual;
    [SerializeField] private GameObject activeVisual;
    [SerializeField] private Animator activeAnimator; // Kéo Animator từ ActiveTrapVisual

    [Header("Settings")]
    [SerializeField] private float cooldownTime = 3f; // Thời gian hồi chiêu
    [SerializeField] private string animationTriggerName = "Slam"; // Tên Trigger trong Animator
    //     
    private bool isOnCooldown = false;
    private Collider2D trapTriggerCollider; // Collider 2D của Parent Object

    // --- Lifecycle Methods ---

    private void Awake()
    {
        // Lấy Collider 2D trên Parent Object
        trapTriggerCollider = GetComponent<Collider2D>();

        // Đảm bảo ban đầu Inactive hiển thị, Active ẩn
        inactiveVisual.SetActive(true);
        activeVisual.SetActive(false);
    }

    // --- Trigger Detection (Phát hiện Player) ---

    // Kích hoạt khi Player (hoặc Object có tag "Player") đi vào hitbox Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra Player và không đang trong thời gian hồi chiêu
        if (other.CompareTag("Player") && !isOnCooldown)
        {
            ActivateTrap();
        }
    }

    // --- Trap Logic (Logic Bẫy) ---

    private void ActivateTrap()
    {
        isOnCooldown = true; // Bắt đầu Cooldown ngay lập tức
        trapTriggerCollider.enabled = false; // Tắt hitbox Trigger để không kích hoạt lại

        // 1. Ẩn hình ảnh chờ
        inactiveVisual.SetActive(false);

        // 2. Hiện hình ảnh hoạt động và bắt đầu animation
        activeVisual.SetActive(true);
        activeAnimator.SetTrigger(animationTriggerName);

        // 3. Đợi cho Animation kết thúc và xử lý Cooldown
        // Thường chúng ta cần biết thời gian animation. 
        // Giả sử animation mất 0.5 giây.
        float animationDuration = GetAnimationDuration(); // Dùng hàm bên dưới để lấy thời gian

        // Bắt đầu coroutine để chờ đợi
        StartCoroutine(DeactivateAndCooldown(animationDuration));
    }

    // Coroutine để xử lý quá trình tắt bẫy và Cooldown
    private System.Collections.IEnumerator DeactivateAndCooldown(float animationDuration)
    {
        // Chờ hết thời gian animation
        yield return new WaitForSeconds(animationDuration);

        // 4. Kết thúc Animation: Ẩn hình ảnh hoạt động
        activeVisual.SetActive(false);

        // Chờ hết thời gian Cooldown
        yield return new WaitForSeconds(cooldownTime);

        // 5. Kết thúc Cooldown: Hiện lại hình ảnh chờ và bật lại Trigger
        inactiveVisual.SetActive(true);
        trapTriggerCollider.enabled = true;
        isOnCooldown = false; // Sẵn sàng kích hoạt lại
    }

    // Hàm lấy thời gian của Animation Clip (cần điều chỉnh)
    private float GetAnimationDuration()
    {
        // Cách tìm thời gian Animation Clip TỰ ĐỘNG
        // Đây là cách chính xác hơn nhưng phức tạp hơn, bạn cần biết tên clip:
        // AnimationClip[] clips = activeAnimator.runtimeAnimatorController.animationClips;
        // foreach (AnimationClip clip in clips)
        // {
        //     if (clip.name == "SlamCeilingTrap_Active_Clip") // Đổi tên clip của bạn
        //     // HOẶC: Nếu chỉ có 1 animation duy nhất, dùng clips[0].length
        //     {
        //         return clip.length;
        //     }
        // }

        // Dùng giá trị thủ công nếu khó lấy tự động (Kiểm tra trong cửa sổ Animation)
        return 0.5f; // Thay thế 0.5f bằng thời lượng thực tế của animation sát thương
    }

    // --- Ghi chú: Xử lý Sát thương ---
    // Hàm gây sát thương cho Player sẽ được gọi TỪ animation clip (thông qua Animation Event)
    // hoặc TẠI ĐIỂM BẮT ĐẦU của animation trong hàm ActivateTrap().
    // Nếu bạn dùng Animation Event, hãy thêm một hàm Public (ví dụ: public void DealDamage())
    // vào script này và gọi nó từ Frame gây sát thương trong animation.
}