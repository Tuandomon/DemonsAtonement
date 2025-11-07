using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 6f;
    public float jumpInterval = 2f;

    [Header("Detection Settings")]
    public float detectionRange = 7.0f;

    [Header("Retreat Behavior")]
    public float retreatDistance = 2.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded;
    private bool playerDetected = false;
    private float jumpTimer;
    // Loại bỏ biến canMove

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        jumpTimer = jumpInterval;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (player == null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (animator != null) animator.SetBool("isRunning", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerDetected = (distanceToPlayer <= detectionRange);

        if (playerDetected)
        {
            float distance = distanceToPlayer;
            float direction = Mathf.Sign(player.position.x - transform.position.x);

            float targetDirection;

            if (distance < retreatDistance)
            {
                targetDirection = -direction;
            }
            else if (distance > retreatDistance + 0.5f)
            {
                targetDirection = direction;
            }
            else
            {
                targetDirection = 0;
            }

            rb.velocity = new Vector2(targetDirection * moveSpeed, rb.velocity.y);

            if (targetDirection != 0)
            {
                spriteRenderer.flipX = targetDirection < 0;
            }
            else
            {
                spriteRenderer.flipX = direction < 0;
            }

            if (animator != null) animator.SetBool("isRunning", targetDirection != 0);

            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimer = jumpInterval;
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (animator != null) animator.SetBool("isRunning", false);
        }

        if (animator != null) animator.SetBool("isJumping", !isGrounded);
    }

}