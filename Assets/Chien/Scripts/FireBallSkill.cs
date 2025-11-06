using UnityEngine;

public class FireBallSkill : MonoBehaviour
{
    [Header("Fireball Settings")]
    public float speed = 8f;
    public float lifetime = 4f;
    public int damage = 50;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isExploding = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Bay về hướng đối tượng đang quay (phải/trái)
        rb.velocity = transform.right * speed;

        // Hủy sau lifetime giây nếu không va chạm
        Destroy(gameObject, lifetime);
    }

    // 🔥 Gọi từ Animation Event tại giây 0.37s
    public void OnFireBallLoop()
    {
        if (!isExploding)
        {
            animator.Play("FireBall_Fly", -1, 0f); // Lặp lại đoạn bay
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploding) return;

        // Va vào Player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Explode();
        }
        // Va vào Ground
        else if (collision.CompareTag("Ground"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        isExploding = true;
        rb.velocity = Vector2.zero; // Dừng di chuyển
        animator.Play("FireBall_Explode", -1, 0.37f); // Chạy từ frame 0.37 tới hết
        Destroy(gameObject, 0.6f); // Hủy sau khi hiệu ứng nổ xong
    }
}
