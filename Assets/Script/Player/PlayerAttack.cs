using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Phím I")]
    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    private float moveInput;

    private PlayerController playerController;

    // 🔥 Hitbox cho đánh thường
    public GameObject attackBox1;
    public GameObject attackBox2;

    // ---------------- Buff đổi skill ----------------
    private bool starBoomActive = false;
    private float starBoomTimer = 0f;
    public GameObject starBoomPrefab;   // Prefab skill đặc biệt
    public Transform firePoint;         // Điểm bắn skill
    public float skillSpeed = 20f;
    // ------------------------------------------------

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        attackBox1.SetActive(false);
        attackBox2.SetActive(false);

        if (firePoint == null)
        {
            GameObject found = GameObject.Find("FirePoint");
            if (found != null) firePoint = found.transform;
            else Debug.LogWarning("Không tìm thấy GameObject tên 'FirePoint'.");
        }
    }

    void Update()
    {
        if (!isAttacking)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * 5f, rb.velocity.y);
        }
        else
        {
            moveInput = 0f;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // Đếm ngược buff
        if (starBoomActive)
        {
            starBoomTimer -= Time.deltaTime;
            if (starBoomTimer <= 0) starBoomActive = false;
        }

        if (Input.GetKeyDown(KeyCode.I) && Time.time >= lastAttackTime + attackCooldown)
        {
            FlipToMoveDirection();

            if (starBoomActive)
                StarBoomAttack(); // bắn skill prefab
            else
                Attack();         // đánh thường
        }
    }

    void FlipToMoveDirection()
    {
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;
    }

    void Attack()
    {
        animator.SetTrigger("attack");
        lastAttackTime = Time.time;
        isAttacking = true;
        playerController.enabled = false;
        Invoke("EndAttack", 1f);
    }

    void StarBoomAttack()
    {
        lastAttackTime = Time.time;

        if (firePoint != null && starBoomPrefab != null)
        {
            GameObject skill = Instantiate(starBoomPrefab, firePoint.position, Quaternion.identity);

            // Xác định hướng Player đang nhìn
            float direction = spriteRenderer.flipX ? -1f : 1f;

            Rigidbody2D rbSkill = skill.GetComponent<Rigidbody2D>();
            if (rbSkill != null)
            {
                rbSkill.velocity = new Vector2(direction * skillSpeed, 0f);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        playerController.enabled = true;
        attackBox1.SetActive(false);
        attackBox2.SetActive(false);
    }

    public void EnableAttackBox1() => attackBox1.SetActive(true);
    public void DisableAttackBox1() => attackBox1.SetActive(false);

    public void EnableAttackBox2() => attackBox2.SetActive(true);
    public void DisableAttackBox2() => attackBox2.SetActive(false);

    // ---------------- API cho ItemChangeStarBoom ----------------
    public void ActivateStarBoom(float duration)
    {
        starBoomActive = true;
        starBoomTimer = duration;
    }
}