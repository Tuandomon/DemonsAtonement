using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;      // Gán đối tượng player trong Inspector
    public float speed = 5f;      // Tốc độ di chuyển của vật
    public float followDistance = 2f; // Khoảng cách giữ với player

    void Update()
    {
        // Tính khoảng cách giữa vật và player
        float distance = Vector3.Distance(transform.position, player.position);

        // Nếu khoảng cách lớn hơn followDistance, di chuyển vật đến gần player
        if (distance > followDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}