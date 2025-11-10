using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    public EnemyHealth enemyHealth;  // 💡 script quản lý máu
    public Slider healthSlider;
    public Transform target;  // Quái (đối tượng cần bám theo)

    void Start()
    {
        if (enemyHealth == null && target != null)
            enemyHealth = target.GetComponent<EnemyHealth>();

        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        if (enemyHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = enemyHealth.maxHealth;
            healthSlider.value = enemyHealth.GetCurrentHealth();
        }
    }

    void LateUpdate()
    {
        // Nếu target đã bị hủy → xóa thanh máu luôn
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (enemyHealth == null || healthSlider == null) return;

        // Nếu quái chết → hủy thanh máu
        if (enemyHealth.GetCurrentHealth() <= 0)
        {
            Destroy(gameObject); // xóa thanh máu khỏi scene
            return;
        }

        // Cập nhật giá trị máu
        healthSlider.value = enemyHealth.GetCurrentHealth();

        // Đặt vị trí thanh máu trùng vị trí quái
        transform.position = target.position + new Vector3(0, 1.5f, 0);

        // Giữ thanh máu không bị xoay theo quái
        transform.rotation = Quaternion.identity;
    }

}
