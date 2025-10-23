using UnityEngine;

public class SentryCannon : MonoBehaviour
{
    [Header("Cannon Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Targeting & Range")]
    public float detectionRange = 8f;        // Bán kính tầm bắn có thể điều chỉnh
    public float rotationSpeed = 5f;         // Tốc độ tháp pháo quay
    public string targetTag = "Player";      // Tag của mục tiêu (Người chơi)

    [Header("Firing Settings")]
    public GameObject projectilePrefab;       // Đạn Prefab
    public Transform barrelTip;               // Vị trí đạn spawn
    public float projectileSpeed = 15f;       // Tốc độ/Lực bắn của đạn
    public float fireRate = 0.5f;             // Tốc độ bắn (0.5f = 2 viên/giây)
    private float nextFireTime;

    [Header("Parts")]
    public Transform aimPivot;                // Transform chịu trách nhiệm xoay súng

    private Transform currentTarget;

    void Start()
    {
        currentHealth = maxHealth;
        nextFireTime = Time.time;

        if (barrelTip == null)
            Debug.LogError("❌ Barrel Tip chưa được gán! Tháp pháo sẽ không bắn được.");
        if (aimPivot == null)
            Debug.LogWarning("⚠️ aimPivot chưa được gán! Súng sẽ xoay cả thân thay vì pivot riêng.");
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        FindTarget();

        if (currentTarget != null)
        {
            AimAtTarget();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    // --- LOGIC SỨC KHỎE ---
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took damage: {damage} | Current HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        // TODO: Thêm hiệu ứng nổ, âm thanh, sprite...
        Destroy(gameObject);
    }

    // --- TÌM VÀ XỬ LÝ MỤC TIÊU ---
    private void FindTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        currentTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D col in hitColliders)
        {
            if (col.CompareTag(targetTag))
            {
                float distanceToTarget = Vector2.Distance(transform.position, col.transform.position);

                if (distanceToTarget < shortestDistance)
                {
                    shortestDistance = distanceToTarget;
                    currentTarget = col.transform;
                }
            }
        }
    }

    private void AimAtTarget()
    {
        if (currentTarget == null) return;

        // Xác định vị trí để xoay (ưu tiên aimPivot, nếu không có thì dùng chính transform)
        Transform pivot = aimPivot != null ? aimPivot : transform;

        // Vector hướng tới mục tiêu
        Vector3 direction = currentTarget.position - pivot.position;

        // Góc hướng mục tiêu
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Quay “mông” về phía mục tiêu => cộng thêm 180 độ
        float desiredAngle = angle + 180f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, desiredAngle);
        pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (projectilePrefab == null || barrelTip == null) return;

        GameObject projectile = Instantiate(projectilePrefab, barrelTip.position, barrelTip.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Giữ hướng bắn chuẩn theo barrelTip.right
            rb.AddForce(-barrelTip.right * projectileSpeed, ForceMode2D.Impulse);
        }
    }

    // --- GIZMOS ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
