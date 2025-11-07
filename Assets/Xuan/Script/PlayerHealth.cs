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

        if (animator != null)
            animator.SetTrigger("GotHit");

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.Stun(1f);

        if (currentHealth <= 0)
            Die();
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            TakeDamage(defaultEnemyDamage);

            PlayerController playerControl = GetComponent<PlayerController>();
            if (playerControl != null)
            {
                playerControl.ApplySlow(defaultSlowPercent, defaultSlowDuration);
            }

            Debug.Log("Player bị Hit Box của Enemy đánh trúng. (Sát thương mặc định)");
        }
    }

    internal void TakeDamage(float damagePerTick)
    {
        int roundedDamage = Mathf.RoundToInt(damagePerTick);
        TakeDamage(roundedDamage);
    }
}