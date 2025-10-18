using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Gọi hàm này khi player nhận sát thương
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player took damage: " + damage + " | Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Gọi hàm này để hồi máu
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player healed: " + amount + " | Current HP: " + currentHealth);
    }

    // Hàm xử lý khi player chết
    void Die()
    {
        Debug.Log("Player has died!");
        // Thêm logic chết như animation, reload scene, v.v.
    }
}