using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mageNPC : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("LightningBall Settings")]
    public Transform firePoint;
    public GameObject lightningBallPrefab;
    public float lightningBallCooldown = 5f;
    private float lastLightningBallTime;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastLightningBallTime = Time.time - lightningBallCooldown; // Cho phép bắn ngay từ đầu
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                MoveTowardsPlayer();
                animator.SetBool("isRunning", true);
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("isRunning", false);

                // Kiểm tra cooldown để bắn LightningBall
                if (Time.time >= lastLightningBallTime + lightningBallCooldown)
                {
                    TryFire();
                    lastLightningBallTime = Time.time;
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
        }

        // Lật hướng enemy theo vị trí Player
        spriteRenderer.flipX = player.position.x < transform.position.x;

        // Cập nhật vị trí firePoint theo hướng
        firePoint.localPosition = spriteRenderer.flipX ? new Vector3(-1f, 0f, 0f) : new Vector3(1f, 0f, 0f);
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    void TryFire()
    {
        animator.SetTrigger("Attack"); // Gọi animation bắn
    }

    // Gọi từ animation event
    public void ShootLightningBall()
    {
        if (lightningBallPrefab != null && firePoint != null)
        {
            GameObject lightningBall = Instantiate(lightningBallPrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            lightningBall.GetComponent<LightningBall>().SetDirection(direction);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        Die();
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}