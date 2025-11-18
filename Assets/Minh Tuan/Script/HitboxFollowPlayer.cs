using UnityEngine;

public class HitboxFollowPlayer : MonoBehaviour
{
    public PlayerController player;   // tham chiếu tới PlayerController
    public Vector3 offsetRight = new Vector3(1f, 0f, 0f); // vị trí khi quay phải
    public Vector3 offsetLeft = new Vector3(-1f, 0f, 0f); // vị trí khi quay trái

    void LateUpdate()
    {
        if (player == null) return;

        // Lấy hướng từ PlayerController qua flipX
        bool facingLeft = player.GetComponent<SpriteRenderer>().flipX;

        // Đặt lại localPosition theo hướng
        transform.localPosition = facingLeft ? offsetLeft : offsetRight;
    }
}