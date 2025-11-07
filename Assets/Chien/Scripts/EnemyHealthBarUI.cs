using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    public EnemyHealth enemyHealth;  // 💡 đổi sang script mới
    public Slider healthSlider;
    public Transform target;  // Quái (đối tượng cần bám theo)

    void Start()
    {
        // Tự lấy script HealthEnemyMage từ target nếu chưa gán
        if (enemyHealth == null && target != null)
            enemyHealth = target.GetComponent<EnemyHealth>();

        // Tự lấy Slider nếu chưa gán
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        // Khởi tạo giá trị thanh máu
        if (enemyHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = enemyHealth.maxHealth;
            healthSlider.value = enemyHealth.GetCurrentHealth();
        }
    }

    void LateUpdate()
    {
        if (enemyHealth == null || healthSlider == null || target == null) return;

        // 🩸 Cập nhật giá trị máu
        healthSlider.value = enemyHealth.GetCurrentHealth();

        // 🧭 Giữ vị trí thanh máu trùng vị trí quái (hoặc cộng offset nếu muốn)
        transform.position = target.position + new Vector3(0, 1.5f, 0); // tùy chỉnh cao/thấp

        // 🔒 Giữ cho thanh máu không bị xoay theo quái
        transform.rotation = Quaternion.identity;
    }
}
