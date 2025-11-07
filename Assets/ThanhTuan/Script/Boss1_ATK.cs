using UnityEngine;

public class Boss1_ATK : MonoBehaviour
{
    [Header("Normal Attack Settings")]
    public GameObject lightPrefab;
    public Transform firePoint;
    public float attackRange = 6f;
    public float attackCooldown = 2f;

    [Header("Triple Skill Settings")]
    public float tripleSkillCooldown = 4f;
    public float angleSpread = 15f;
    private float nextTripleSkillTime = 0f;
    private bool isUsingTripleSkill = false;

    [Header("FireBall Skill Settings")]
    public GameObject fireBallPrefab;
    public float fireBallCooldown = 8f;
    private float nextFireBallTime = 0f;
    private bool isUsingFireBall = false;

    [Header("References")]
    public Animator animator;
    private Transform player;
    private float nextAttackTime;
   

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
 

        nextFireBallTime = Time.time + fireBallCooldown;
        nextTripleSkillTime = Time.time + tripleSkillCooldown;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(-0.8f, 0.8f, 1f);
            else
                transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            if (isUsingTripleSkill || isUsingFireBall) return;

   
            if (animator.GetBool("isRunning")) return;

            if (Time.time >= nextFireBallTime)
            {
                isUsingFireBall = true;
                animator.SetTrigger("attack");
                Invoke(nameof(ShootFireBall), 0.6f);
                nextFireBallTime = Time.time + fireBallCooldown;
                return;
            }

            if (Time.time >= nextTripleSkillTime)
            {
                isUsingTripleSkill = true;
                animator.SetTrigger("attack");
                nextTripleSkillTime = Time.time + tripleSkillCooldown;
                Invoke(nameof(ShootTriple), 0.4f);
                return;
            }

            if (Time.time >= nextAttackTime)
            {
                animator.SetTrigger("attack");
                Invoke(nameof(Shoot), 0.4f);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            animator.ResetTrigger("attack");
            animator.Play("Idle_Linh2");
        }
    }

    // Loại bỏ hàm BatDauTanCong và KetThucTanCong

    public void Shoot()
    {
        if (isUsingTripleSkill || isUsingFireBall) return;
        if (lightPrefab == null || firePoint == null || player == null) return;

        GameObject light = Instantiate(lightPrefab, firePoint.position, Quaternion.identity);

        Vector3 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        light.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = light.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dir * 8f;
    }

    private void ShootTriple()
    {
        if (lightPrefab == null || firePoint == null || player == null) return;

        Vector3 baseDir = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        float[] spreadAngles = { -angleSpread, 0f, angleSpread };

        foreach (float spread in spreadAngles)
        {
            GameObject light = Instantiate(lightPrefab, firePoint.position, Quaternion.Euler(0, 0, baseAngle + spread));
            Rigidbody2D rb = light.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootDir = Quaternion.Euler(0, 0, spread) * baseDir;
                rb.velocity = shootDir * 8f;
            }
        }

        Invoke(nameof(ResetTripleSkill), 1f);
        Debug.Log("Mage used Triple Light Skill!");
    }

    private void ResetTripleSkill() => isUsingTripleSkill = false;

    private void ShootFireBall()
    {
        if (fireBallPrefab == null || firePoint == null || player == null) return;

        GameObject fireBall = Instantiate(fireBallPrefab, firePoint.position, Quaternion.identity);

        Vector3 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        fireBall.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = fireBall.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dir * 10f;

        Invoke(nameof(ResetFireBallSkill), 1f);
        Debug.Log("Mage used FireBall!");
    }

    private void ResetFireBallSkill() => isUsingFireBall = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}