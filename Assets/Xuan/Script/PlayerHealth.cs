using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Settings")]
    public Image healthFillImage;         // Gán Image của thanh máu
    public Gradient healthGradient;       // Gradient màu máu

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // Tự động tìm Image thanh máu nếu chưa gán
        if (healthFillImage == null)
        {
            GameObject healthBarObj = GameObject.Find("HealthBarFill");
            if (healthBarObj != null)
            {
                healthFillImage = healthBarObj.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameObject 'HealthBarFill' trong scene!");
            }
        }

        UpdateHealthUI();
    }

    void Update()
    {
        // Test giảm máu khi nhấn phím O
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(5); // Giảm 5 máu mỗi lần nhấn
        }
    }

    // Gọi hàm này khi player nhận sát thương
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (animator != null)
            animator.SetTrigger("GotHit");

        // Gây stun thông qua PlayerController
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.Stun(0.4f); // làm choáng 0.2 giây

        if (currentHealth <= 0)
            Die();
    }

    // Gọi hàm này để hồi máu
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player healed: " + amount + " | Current HP: " + currentHealth);

        UpdateHealthUI();

    }

    // Cập nhật UI thanh máu
    void UpdateHealthUI()
    {
        if (healthFillImage != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthFillImage.fillAmount = fillAmount;
            healthFillImage.color = healthGradient.Evaluate(fillAmount);
        }
    }

    // Hàm xử lý khi player chết
    void Die()
    {
        Debug.Log("Player has died!");
        // Thêm logic chết như animation, reload scene, v.v.
    }

    internal void TakeDamage(float damagePerTick)
    {
        throw new NotImplementedException();
    }
}