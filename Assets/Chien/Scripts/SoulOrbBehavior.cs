using UnityEngine;

public class SoulOrbBehavior : MonoBehaviour
{
    [Header("Target Player")]
    public Transform player;          // Player sẽ bay về
    public float stayTime = 3f;       // Thời gian đứng yên
    public float moveSpeed = 5f;      // Tốc độ bay về Player
    public float reachDistance = 0.5f; // Khoảng cách chạm Player để biến mất

    private float elapsed = 0f;
    private bool isMoving = false;

    void Update()
    {
        if (player == null) return;

        elapsed += Time.deltaTime;

        // Sau khi stayTime hết, bắt đầu bay về player
        if (!isMoving && elapsed >= stayTime)
            isMoving = true;

        if (isMoving)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, player.position) <= reachDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.transform == player)
        {
            Destroy(gameObject);
        }
    }
}