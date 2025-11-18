using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Cài đặt máu")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Cho phép destroy object khi chết")]
    public bool allowDestroy = true; // nếu false thì EnemyHealth không destroy

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} đã bị tiêu diệt!");

        if (allowDestroy)
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
