using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform player;  // Gán Player vào đây trong Inspector
    public float smoothSpeed = 2f; // tốc độ theo dõi mượt

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
