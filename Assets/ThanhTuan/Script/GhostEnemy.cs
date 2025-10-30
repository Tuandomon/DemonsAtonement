using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    // CÁC BIẾN CÓ THỂ ĐIỀU CHỈNH TRONG UNITY INSPECTOR

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 1.5f;
    public Transform player;

    [Header("Fixed Interval Damage Settings")]
    public float damagePerTick = 5f; // << Mức SÁT THƯƠNG cố định mỗi lần (tick)
    public float damageInterval = 1f; // << Khoảng thời gian giữa các lần gây sát thương (Mỗi giây)

    [Header("Damage Area Settings")]
    public float damageAreaRadius = 2f;
    public GameObject damageAreaVisualPrefab;

    private float nextDamageTime;
    private GameObject currentDamageAreaVisual;

    void Start()
    {
        nextDamageTime = Time.time;

        CreateDamageAreaCollider();
        DisplayDamageAreaVisual();
    }

    void Update()
    {
        MoveTowardsPlayer();
    }

    // --- DI CHUYỂN (Giữ nguyên) ---
    void MoveTowardsPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    // --- CÁC HÀM TẠO VÙNG VÀ HIỂN THỊ (Giữ nguyên) ---
    void CreateDamageAreaCollider()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        sphereCollider.isTrigger = true;
        sphereCollider.radius = damageAreaRadius;
    }

    void DisplayDamageAreaVisual()
    {
        if (damageAreaVisualPrefab != null)
        {
            if (currentDamageAreaVisual != null)
            {
                Destroy(currentDamageAreaVisual);
            }
            currentDamageAreaVisual = Instantiate(damageAreaVisualPrefab, transform.position, Quaternion.identity, transform);
            currentDamageAreaVisual.transform.localScale = Vector3.one * (damageAreaRadius * 2);
            currentDamageAreaVisual.name = "GhostDamageAreaVisual";
        }
    }

    // --- LOGIC GÂY SÁT THƯƠNG (Điều chỉnh công thức) ---
    private void OnTriggerStay(Collider other)
    {
        // Kiểm tra xem vật thể va chạm có phải là Player không
        if (other.CompareTag("Player"))
        {
            // Kiểm tra xem đã đến lúc gây sát thương tiếp theo chưa
            if (Time.time >= nextDamageTime)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Gây sát thương cố định (damagePerTick = 5f)
                    playerHealth.TakeDamage(damagePerTick);

                    Debug.Log($"Player bị sát thương cố định: {damagePerTick} HP.");
                }

                // Cập nhật thời gian gây sát thương tiếp theo sau 1 giây (damageInterval = 1f)
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}