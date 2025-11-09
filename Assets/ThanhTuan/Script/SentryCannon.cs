using UnityEngine;

public class SentryCannon : MonoBehaviour
{
    [Header("Cannon Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Targeting & Range")]
    public float detectionRange = 8f;
    public float rotationSpeed = 5f;
    public string targetTag = "Player";

    [Header("Firing Settings")]
    public GameObject projectilePrefab;
    public Transform barrelTip;
    public float projectileSpeed = 15f;
    public float fireRate = 0.5f;
    private float nextFireTime;

    [Header("Audio Settings")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    [Header("Parts")]
    public Transform aimPivot;

    private Transform currentTarget;

    void Start()
    {
        currentHealth = maxHealth;
        nextFireTime = Time.time;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (barrelTip == null)
            Debug.LogError("Barrel Tip chưa được gán! Tháp pháo sẽ không bắn được.");
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
        Destroy(gameObject);
    }

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

        Transform pivot = aimPivot != null ? aimPivot : transform;

        Vector3 direction = currentTarget.position - pivot.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

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
            rb.AddForce(-barrelTip.right * projectileSpeed, ForceMode2D.Impulse);
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}