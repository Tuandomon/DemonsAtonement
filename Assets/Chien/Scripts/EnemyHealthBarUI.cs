using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    public EnemyHealth enemyHealth;
    public Slider healthSlider;
    public Transform target;  // sói (đối tượng cần bám theo)

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
        if (enemyHealth == null || healthSlider == null || target == null) return;

        // cập nhật giá trị máu
        healthSlider.value = enemyHealth.GetCurrentHealth();

        // luôn giữ vị trí gốc của thanh máu (không cộng offset)
        transform.position = target.position;

        // giữ cho thanh máu không bị xoay theo sói
        transform.rotation = Quaternion.identity;
    }
}
