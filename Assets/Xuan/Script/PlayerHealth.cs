using System.Collections;
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

    public static object Instance { get; internal set; }

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
        if (currentHealth <= 0) return; // tránh gọi Die nhiều lần

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (animator != null)
            animator.SetTrigger("GotHit");

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.Stun(1f);

        if (currentHealth <= 0)
            Die();
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

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

        if (animator != null)
            animator.SetTrigger("Die");

        StartCoroutine(DieAndRespawn());
    }

    IEnumerator DieAndRespawn()
    {
        // Tắt điều khiển nếu cần
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        yield return new WaitForSeconds(1f); // giữ animation Die trong 1 giây

        currentHealth = maxHealth;
        UpdateHealthUI();

        // Hồi sinh tại checkpoint
        RespawnManager.Instance.Respawn(gameObject);

        // Reset animation Die
        if (animator != null)
            animator.ResetTrigger("Die");

        // Bật lại điều khiển
        if (controller != null)
        {
            controller.ResetState(); // nếu có
            controller.enabled = true; // ✅ bật lại sau khi hồi sinh
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            TakeDamage(defaultEnemyDamage);

            PlayerController playerControl = GetComponent<PlayerController>();
            if (playerControl != null)
                playerControl.ApplySlow(defaultSlowPercent, defaultSlowDuration);

            Debug.Log("Player bị Enemy đánh trúng.");
        }

        if (collision.CompareTag("HealthItem"))
        {
            HealthItem item = collision.GetComponent<HealthItem>();
            if (item != null)
            {
                AddHealth(item.healAmount);
                item.OnCollected();
            }
        }
    }

    internal void TakeDamage(float damagePerTick)
    {
        int roundedDamage = Mathf.RoundToInt(damagePerTick);
        TakeDamage(roundedDamage);
    }
}