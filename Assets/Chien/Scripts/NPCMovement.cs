using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    [Header("Thiết lập di chuyển")]
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Thời gian đứng im")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    [Range(0f, 1f)] public float idleChance = 0.02f; // tỉ lệ tạm dừng (thấp = hiếm khi đứng)

    private Animator animator;
    private bool movingRight;
    private bool isIdle = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (leftPoint == null || rightPoint == null)
        {
            Debug.LogWarning($"{name} chưa gán LeftPoint hoặc RightPoint!");
            enabled = false;
            return;
        }

        // Ngẫu nhiên hướng ban đầu
        movingRight = Random.value > 0.5f;
        UpdateFacingDirection();
    }

    void Update()
    {
        if (isIdle) return;

        Move();

        // Thỉnh thoảng tạm dừng 1 chút
        if (Random.value < idleChance * Time.deltaTime)
        {
            StartCoroutine(IdleRoutine());
        }
    }

    void Move()
    {
        float direction = movingRight ? 1f : -1f;
        Vector3 move = new Vector3(direction * moveSpeed * Time.deltaTime, 0f, 0f);
        transform.position += move;

        animator.SetFloat("Speed", Mathf.Abs(moveSpeed));

        // Kiểm tra và đảo hướng khi đến giới hạn
        if (movingRight && transform.position.x >= rightPoint.position.x)
        {
            transform.position = new Vector3(rightPoint.position.x, transform.position.y, transform.position.z);
            movingRight = false;
            UpdateFacingDirection();
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x)
        {
            transform.position = new Vector3(leftPoint.position.x, transform.position.y, transform.position.z);
            movingRight = true;
            UpdateFacingDirection();
        }
    }

    IEnumerator IdleRoutine()
    {
        isIdle = true;
        animator.SetBool("IsIdle", true);
        animator.SetFloat("Speed", 0);

        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

        // Đôi khi đổi hướng khi dừng
        if (Random.value > 0.5f)
        {
            movingRight = !movingRight;
            UpdateFacingDirection();
        }

        animator.SetBool("IsIdle", false);
        isIdle = false;
    }

    void UpdateFacingDirection()
    {
        // Xoay NPC bằng rotation Y
        transform.rotation = Quaternion.Euler(0, movingRight ? -180 : 0, 0);
    }

    // Vẽ phạm vi trong Scene để dễ kiểm tra
    void OnDrawGizmosSelected()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        }
    }
}
