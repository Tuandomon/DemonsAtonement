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

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

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

        UpdateHealthUI(); // <<< ĐÃ FIX LỖI CS0103

        if (animator != null)
            animator.SetTrigger("GotHit");

        // Gây stun thông qua PlayerController
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.Stun(0.4f);

        if (currentHealth <= 0)
            Die();
    }

    // <<< ĐỊNH NGHĨA HÀM BỊ THIẾU ĐÃ ĐƯỢC THÊM VÀO >>>
    void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthFillImage.fillAmount = fillAmount;

            if (healthGradient != null)
            {
                healthFillImage.color = healthGradient.Evaluate(fillAmount);
            }
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
    }

    // LOGIC NHẬN SÁT THƯƠNG TỪ HIT BOX CỦA ENEMY
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            EnemyController enemyControl = collision.GetComponentInParent<EnemyController>();

            if (enemyControl != null)
            {
                TakeDamage(enemyControl.attackDamage);

                PlayerController playerControl = GetComponent<PlayerController>();
                if (playerControl != null)
                {
                    // Đã fix lỗi CS0106 bằng cách đảm bảo biến trong EnemyController là public
                    playerControl.ApplySlow(enemyControl.slowPercent, enemyControl.slowDuration);
                }

                Debug.Log("Player bị Hit Box của Enemy đánh trúng.");
            }
        }
    }

    // Giữ lại hàm này
    internal void TakeDamage(float damagePerTick)
    {
        // Logic cho damage/tick
    }
}