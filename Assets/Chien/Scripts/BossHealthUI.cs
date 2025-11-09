using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    public EnemyHealth bossHealth; // Script máu của boss
    public Slider healthSlider;
    public Transform target;        // Boss (đối tượng cần bám theo)

    [Header("Offset vị trí")]
    public Vector3 offset = new Vector3(0, 2f, 0); // Thanh máu nằm trên đầu boss

    void Start()
    {
        // Tự lấy script EnemyHealth từ target nếu chưa gán
        if (bossHealth == null && target != null)
            bossHealth = target.GetComponent<EnemyHealth>();

        // Tự lấy Slider nếu chưa gán
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        // Khởi tạo giá trị thanh máu
        if (bossHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.GetCurrentHealth();
        }
    }

    void LateUpdate()
    {
        if (bossHealth == null || healthSlider == null || target == null) return;

        // Cập nhật giá trị máu
        healthSlider.value = bossHealth.GetCurrentHealth();

        // Giữ vị trí thanh máu trùng vị trí boss + offset
        transform.position = target.position + offset;

        // Giữ cho thanh máu không bị xoay theo boss
        transform.rotation = Quaternion.identity;
    }
}
