using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Cài đặt kỹ năng")]
    [Header("Phím W + U")]
    [Tooltip("Thời gian hồi chiêu (giây)")]
    public float skillCooldown = 3f;

    [Tooltip("Phạm vi tìm enemy")]
    public float skillRange = 5f;

    [Tooltip("Sát thương gây ra")]
    public int skillDamage = 50;

    [Tooltip("Prefab hiệu ứng kỹ năng")]
    public GameObject skillEffectPrefab;

    [Tooltip("Layer của enemy")]
    public LayerMask enemyLayer;

    private float lastSkillTime = -Mathf.Infinity;

    void Update()
    {
        // Kích hoạt khi giữ W và nhấn U
        if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.U))
        {
            if (Time.time >= lastSkillTime + skillCooldown)
            {
                TryActivateSkill();
            }
            else
            {
                Debug.Log("Skill đang hồi chiêu...");
            }
        }
    }

    void TryActivateSkill()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, skillRange);

        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestEnemy = col.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            ActivateSkill(closestEnemy);
            lastSkillTime = Time.time;
        }
        else
        {
            Debug.Log("Không có enemy hợp lệ trong vùng, skill không được kích hoạt.");
        }
    }

    void ActivateSkill(Transform target)
    {
        Debug.Log("Kích hoạt skill lên: " + target.name);

        if (skillEffectPrefab != null)
        {
            Instantiate(skillEffectPrefab, target.position, Quaternion.identity);
        }

        EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(skillDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, skillRange);
    }
}