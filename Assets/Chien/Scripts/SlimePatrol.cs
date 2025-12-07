using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Attack Settings")]
    public float chaseSpeed = 3.5f;
    public float detectRange = 6f;
    public float attackRange = 1.2f;
    public float attackCooldown = 0.4f;

    [Header("Audio Settings")]
    public AudioClip attackSound;

    [Header("Movement Sound Settings")]
    public AudioClip walkSound;
    public float walkSoundInterval = 0.8f;
    private float lastWalkSoundTime = 0f;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float baseWalkVolume = 0.45f;

    // ⭐ Giảm volume attack xuống nhẹ cho em
    [Range(0f, 1f)] public float baseAttackVolume = 0.4f;

    [Header("Sound Range")]
    public float soundRange = 5f;

    [Header("References")]
    public Animator animator;
    public Transform player;

    [Header("Distance Settings")]
    public float tooCloseDistance = 0.4f;
    public float idealAttackDistance = 1.2f;
    public float backstepDistance = 1f;
    public float backstepSpeed = 4f;
    private bool isBackingUp = false;

    private Rigidbody2D rb;

    // ⭐ 2 audio source riêng
    private AudioSource audioSourceWalk;
    private AudioSource audioSourceAttack;

    private bool isWaiting = false;
    private int moveDirection = 1;
    private float lastAttackTime = 0f;

    private enum FoxState { Patrol, Chase, Attack }
    private FoxState currentState = FoxState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = animator != null ? animator : GetComponent<Animator>();

        audioSourceWalk = gameObject.AddComponent<AudioSource>();
        audioSourceWalk.playOnAwake = false;
        audioSourceWalk.loop = false;

        audioSourceAttack = gameObject.AddComponent<AudioSource>();
        audioSourceAttack.playOnAwake = false;
        audioSourceAttack.loop = false;

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = transform.position.x < mid ? 1 : -1;
        }

        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject e in allEnemies)
            {
                if (e == gameObject) continue;
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        bool playerInPatrolRange = (leftPoint != null && rightPoint != null) &&
                                   player.position.x >= leftPoint.position.x &&
                                   player.position.x <= rightPoint.position.x;

        if (playerInPatrolRange)
        {
            if (dist <= attackRange && Time.time - lastAttackTime >= attackCooldown)
                currentState = FoxState.Attack;
            else if (dist <= detectRange)
                currentState = FoxState.Chase;
            else
                currentState = FoxState.Patrol;
        }
        else
        {
            currentState = FoxState.Patrol;
        }

        switch (currentState)
        {
            case FoxState.Patrol: PatrolState(); break;
            case FoxState.Chase: ChaseState(); break;
            case FoxState.Attack: AttackState(); break;
        }
    }

    void PatrolState()
    {
        if (isWaiting)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            FadeOutWalkImmediate();
            return;
        }

        if (leftPoint == null || rightPoint == null) return;

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        HandleWalkSound();
        UpdateFacingDirection(rb.velocity.x);

        if ((moveDirection > 0 && transform.position.x >= rightPoint.position.x) ||
            (moveDirection < 0 && transform.position.x <= leftPoint.position.x))
        {
            StartCoroutine(WaitAndTurn());
        }
    }

    void ChaseState()
    {
        FadeOutWalkImmediate();

        if (isBackingUp) return;

        float distance = Vector2.Distance(transform.position, player.position);
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        if (distance < tooCloseDistance)
        {
            if (!isBackingUp)
                StartCoroutine(BackstepRoutine(-dir));
            return;
        }

        if (distance > idealAttackDistance)
        {
            rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            HandleWalkSound();
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            FadeOutWalkImmediate();
        }

        UpdateFacingDirection(rb.velocity.x);
    }

    void AttackState()
    {
        FadeOutWalkImmediate();
        rb.velocity = Vector2.zero;

        float distance = Vector2.Distance(transform.position, player.position);
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        if (distance < tooCloseDistance && !isBackingUp)
        {
            StartCoroutine(BackstepRoutine(-dir));
        }

        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
            return; // ⭐ KHÔNG PHÁT ÂM Ở ĐÂY NỮA
        }

        UpdateFacingDirection(0);
    }

    void HandleWalkSound()
    {
        if (walkSound == null || audioSourceWalk == null || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (Mathf.Abs(rb.velocity.x) < 0.05f)
        {
            FadeOutWalkImmediate();
            return;
        }

        if (distance > soundRange)
        {
            FadeOutWalk();
            return;
        }

        float volumePercent = 1f - (distance / soundRange);
        float finalVolume = Mathf.Clamp01(volumePercent) * baseWalkVolume;

        audioSourceWalk.volume = finalVolume;

        if (Time.time - lastWalkSoundTime >= walkSoundInterval)
        {
            audioSourceWalk.clip = walkSound;
            audioSourceWalk.Play();
            lastWalkSoundTime = Time.time;
        }
    }

    void FadeOutWalk()
    {
        if (audioSourceWalk.isPlaying)
        {
            audioSourceWalk.volume -= Time.deltaTime * 1.5f;

            if (audioSourceWalk.volume <= 0.05f)
            {
                audioSourceWalk.Stop();
                audioSourceWalk.volume = baseWalkVolume;
            }
        }
    }

    void FadeOutWalkImmediate()
    {
        if (audioSourceWalk.isPlaying)
            audioSourceWalk.Stop();

        audioSourceWalk.volume = baseWalkVolume;
    }

    // ⭐ Animation Event gọi hàm này
    public void DealDamage()
    {
        // ⭐ PHÁT ÂM THANH TẤN CÔNG TẠI ĐÂY
        PlayAttackSound();

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(100);
        }
    }

    // ⭐ Hàm âm thanh attack CHỈ chạy khi Animation Event gọi
    public void PlayAttackSound()
    {
        if (attackSound == null || audioSourceAttack == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > soundRange) return;

        float volumePercent = 1f - (distance / soundRange);
        float finalVolume = Mathf.Clamp01(volumePercent) * baseAttackVolume;

        audioSourceAttack.PlayOneShot(attackSound, finalVolume);
    }

    IEnumerator BackstepRoutine(float dir)
    {
        isBackingUp = true;
        animator.SetFloat("Speed", backstepSpeed);

        float startX = transform.position.x;
        float target = startX + dir * backstepDistance;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * backstepSpeed;
            float newX = Mathf.Lerp(startX, target, t);
            rb.MovePosition(new Vector2(newX, transform.position.y));
            yield return null;
        }

        animator.SetFloat("Speed", 0);
        isBackingUp = false;
    }

    IEnumerator WaitAndTurn()
    {
        isWaiting = true;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
        FadeOutWalkImmediate();

        yield return new WaitForSeconds(waitTime);

        moveDirection *= -1;
        isWaiting = false;
    }

    void UpdateFacingDirection(float moveDir)
    {
        if (moveDir > 0.05f) transform.rotation = Quaternion.Euler(0, 180f, 0);
        else if (moveDir < -0.05f) transform.rotation = Quaternion.Euler(0, 0f, 0);
    }

    void OnDrawGizmosSelected()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }
}
