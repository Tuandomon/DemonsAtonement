using UnityEngine;

public class SpikeMoving : MonoBehaviour
{
    [Header("Điểm mục tiêu Spike sẽ đi lên đến")]
    public Transform targetPoint;

    [Header("Tốc độ di chuyển")]
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private bool playerInside = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (playerInside)
        {
            // Di chuyển lên target point
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPoint.position,
                moveSpeed * Time.deltaTime
            );
        }
        // ❌ Không để else → tránh chạy MoveTowards khi đã reset ngay lập tức
    }

    // Khi Player vào vùng trigger → gai đi lên
    public void Activate()
    {
        playerInside = true;
    }

    // Khi Player ra → gai trở về ngay vị trí ban đầu
    public void Deactivate()
    {
        playerInside = false;
        transform.position = startPosition;  // Trở lại tức thời
    }
}
