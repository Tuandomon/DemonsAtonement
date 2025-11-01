using System.Collections;
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
        StartCoroutine(ApplySkillOverTime(target));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, skillRange);
    }

    IEnumerator ApplySkillOverTime(Transform target)
    {
        float duration = 10f;
        float tickInterval = 1f; // mỗi giây gây sát thương
        int tickDamage = skillDamage / 10; // chia đều sát thương cho 10 giây

        GameObject effectInstance = null;
        if (skillEffectPrefab != null)
        {
            effectInstance = Instantiate(skillEffectPrefab, target.position, Quaternion.identity, target);
        }

        EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(tickDamage);
            }

            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        if (effectInstance != null)
        {
            Destroy(effectInstance);
        }
    }
}