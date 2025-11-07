using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    public EnemyHealth enemyHealth;     // Script máu của quái
    public Slider healthSlider;         // Slider hiển thị máu
    public Transform followTarget;      // Object để thanh máu theo (ví dụ: đầu sói)

    private Quaternion fixedRotation;   // Giữ canvas không xoay theo quái

    void Start()
    {
        if (enemyHealth == null)
            Debug.LogWarning("⚠️ Chưa gán EnemyHealth trong Inspector!");

        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        if (enemyHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = enemyHealth.maxHealth;
            healthSlider.value = enemyHealth.maxHealth;
        }

        // Giữ hướng gốc
        fixedRotation = transform.rotation;

        if (followTarget == null && enemyHealth != null)
            followTarget = enemyHealth.transform; // fallback
    }

    void LateUpdate()
    {
        if (enemyHealth == null || healthSlider == null || followTarget == null)
            return;

        // Theo dõi vị trí của followTarget (đầu sói hoặc vị trí mong muốn)
        transform.position = followTarget.position;

        // Giữ hướng cố định (tránh xoay theo sói)
        transform.rotation = fixedRotation;

        // Cập nhật thanh máu
        healthSlider.value = Mathf.Clamp(enemyHealth.GetCurrentHealth(), 0, enemyHealth.maxHealth);

        // Khi quái chết thì xoá luôn thanh máu
        if (enemyHealth.GetCurrentHealth() <= 0)
            Destroy(gameObject);
    }
}
