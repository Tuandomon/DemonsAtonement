using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 1000f; // Máu tối đa của Boss
    private float currentHealth;
    public GameObject bossDeathEffectPrefab; // Prefab hiệu ứng khi boss chết (Optional)

    [Header("UI Settings")]
    public Slider healthSlider; // Kéo thả thanh Slider UI vào đây
    public GameObject healthBarCanvas; // Kéo thả Canvas chứa thanh máu vào đây (Optional)

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Phương thức để Boss nhận sát thương
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Đảm bảo máu không xuống dưới 0
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Cập nhật thanh máu UI 
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthBarCanvas != null)
        {
            // Hiển thị thanh máu khi bắt đầu nhận sát thương 
            healthBarCanvas.SetActive(currentHealth < maxHealth && currentHealth > 0);
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " đã bị tiêu diệt!");

        // 1. Kích hoạt hiệu ứng (nếu có)
        if (bossDeathEffectPrefab != null)
        {
            Instantiate(bossDeathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Vô hiệu hóa hoặc hủy đối tượng Boss
        Destroy(gameObject);

        // Thêm logic game over/win tại đây
    }
}