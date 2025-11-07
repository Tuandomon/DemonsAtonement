using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Settings")]
    public Image healthFillImage;
    public Gradient healthGradient;

    [Header("Default Enemy Damage")]
    public int defaultEnemyDamage = 10;
    public float defaultSlowPercent = 0.4f;
    public float defaultSlowDuration = 2.0f;

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // Tự tìm UI nếu chưa gán
        if (healthFillImage == null)
        {
            GameObject healthBarObj = GameObject.Find("HealthBarFill");
            if (healthBarObj != null)
            {
                healthFillImage = healthBarObj.GetComponent<Image>();
            }
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        // Gọi animation bị đánh
        if (animator != null)
            animator.SetTrigger("GotHit");

        // Làm chậm hoặc choáng
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.Stun(1f);

        if (currentHealth <= 0)
            Die();
    }

    public void AddHealth(int amount)
    {
        // Thêm máu và cập nhật UI
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        // Có thể thêm animation hoặc hiệu ứng hồi máu tại đây
        if (animator != null)
            animator.SetTrigger("Heal");
    }

    void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthFillImage.fillAmount = fillAmount;

            if (healthGradient != null)
                healthFillImage.color = healthGradient.Evaluate(fillAmount);
        }
    }

    void Die()
    {
        Debug.Log("Player đã chết!");
        // Gợi ý: Gọi animation chết hoặc reload scene ở đây
        if (animator != null)
            animator.SetTrigger("Die");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            TakeDamage(defaultEnemyDamage);

            PlayerController playerControl = GetComponent<PlayerController>();
            if (playerControl != null)
                playerControl.ApplySlow(defaultSlowPercent, defaultSlowDuration);

            Debug.Log("Player bị Enemy đánh trúng (sát thương mặc định).");
        }

        // 🩹 Thêm phần này để player ăn item hồi máu
        if (collision.CompareTag("HealthItem"))
        {
            HealthItem item = collision.GetComponent<HealthItem>();
            if (item != null)
            {
                AddHealth(item.healAmount);
                item.OnCollected(); // Gọi hàm xử lý biến mất / hiệu ứng
            }
        }
    }

    // Overload cho damage theo thời gian
    internal void TakeDamage(float damagePerTick)
    {
        int roundedDamage = Mathf.RoundToInt(damagePerTick);
        TakeDamage(roundedDamage);
    }
}
