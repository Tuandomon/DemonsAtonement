using UnityEngine;

public class BossAttack : MonoBehaviour
{
    // Các biến cần gán trong Inspector
    [Header("Attack Settings")]
    public float attackRange = 3f; // Tầm tấn công (điều chỉnh được)
    public float attackCooldown = 1.5f; // Thời gian hồi chiêu

    [Header("References")]
    public Transform player;
    public Animator animator; // Dùng để kích hoạt animation!

    private float nextAttackTime = 0f;
    private const int ATTACK_DAMAGE = 20;

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                // *** Dòng code KÍCH HOẠT ANIMATION: ***
                animator.SetTrigger("Attack"); // Kích hoạt Trigger "Attack" trong Animator

                nextAttackTime = Time.time + attackCooldown;

                // LƯU Ý: Vẫn nên dùng Animation Event cho việc gây sát thương 
                // thay vì gọi ngay lập tức ở đây.
            }
        }
    }

    // Hàm này được gọi bởi Animation Event trong trạng thái "Attack"
    public void DealDamageToPlayer()
    {
        // Kiểm tra lại lần nữa khi animation chạy đến frame gây sát thương
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            // PlayerHealth là component trên Player
            player.GetComponent<PlayerHealth>()?.TakeDamage(ATTACK_DAMAGE);
            // Debug.Log("Boss gây " + ATTACK_DAMAGE + " sát thương!");
        }
    }
}