using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Phím S + J")]
    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    private float moveInput;

    private PlayerController playerController;

    // 🔥 Thêm 2 box gây damage
    public GameObject attackBox1;
    public GameObject attackBox2;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        // 🔒 Tắt box ban đầu
        attackBox1.SetActive(false);
        attackBox2.SetActive(false);
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

        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J) && Time.time >= lastAttackTime + attackCooldown)
        {
            FlipToMoveDirection(); // thay vì FlipToMouse
            Attack();
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
        Invoke("EndAttack", 1f); // hoặc dùng Animation Event
    }

    public void EndAttack()
    {
        Debug.Log("EndAttack được gọi");
        isAttacking = false;
        playerController.enabled = true;
        attackBox1.SetActive(false);
        attackBox2.SetActive(false);
    }

    // 🥊 Gọi từ Animation Event để kích hoạt box 1
    public void EnableAttackBox1() => attackBox1.SetActive(true);
    public void DisableAttackBox1() => attackBox1.SetActive(false);

    // 🥊 Gọi từ Animation Event để kích hoạt box 2
    public void EnableAttackBox2() => attackBox2.SetActive(true);
    public void DisableAttackBox2() => attackBox2.SetActive(false);
}