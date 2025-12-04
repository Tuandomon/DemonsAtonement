using UnityEngine;
using System.Collections;

public class PlayerDeathRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint; // Vị trí hồi sinh
    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Nếu chưa gán respawnPoint, mặc định hồi sinh tại vị trí ban đầu
        if (respawnPoint == null)
            respawnPoint = transform;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu chạm vào gai (tag "Spike")
        if (collision.gameObject.CompareTag("Spike") && !isDead)
        {
            Die();
        }
    }

    // Hoặc dùng OnTriggerEnter2D nếu gai là trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike") && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Tắt di chuyển player (nếu có script điều khiển riêng)
        GetComponent<PlayerController>().enabled = false;

        // Tắt vật lý (nếu cần)
        if (rb != null)
            rb.velocity = Vector2.zero;

        // Kích hoạt animation chết (nếu có)
        if (animator != null)
            animator.SetTrigger("Die");

        // Gọi hàm hồi sinh sau 1 giây (hoặc thời gian animation chết)
        StartCoroutine(Respawn(1f));
    }

    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Đặt player tại vị trí hồi sinh
        transform.position = respawnPoint.position;

        // Bật lại di chuyển
        GetComponent<PlayerController>().enabled = true;

        // Reset trạng thái
        isDead = false;

        // Reset animation (nếu có)
        if (animator != null)
            animator.SetTrigger("Respawn");
    }

    // Hàm để thay đổi checkpoint khi player chạm vào checkpoint mới
    public void SetNewRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
    }
}