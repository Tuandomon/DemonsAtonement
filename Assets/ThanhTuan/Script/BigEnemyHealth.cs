using UnityEngine;

public class BigEnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 500; // 500 Máu cho Big Enemy
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm nhận sát thương (gọi từ đạn/vũ khí của Player)
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Xử lý cái chết của Big Enemy
    void Die()
    {
        Debug.Log(gameObject.name + " (BIG ENEMY) đã bị tiêu diệt!");

        // Vô hiệu hóa Controller
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null) controller.enabled = false;

        // Vô hiệu hóa Rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        // Xóa GameObject sau 1.5s
        Destroy(gameObject, 1.5f);
    }
}