using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPickup : MonoBehaviour
{
    [Header("Dash Settings")]
    public bool temporary = false; // nếu true thì dash chỉ tồn tại trong vài giây
    public float dashDuration = 5f; // thời gian giữ dash nếu là tạm thời

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // bật khả năng dash
        

            // hiệu ứng hoặc âm thanh (tùy thêm)
            Debug.Log("Player picked up Dash Item!");

            // xóa item khỏi map
            Destroy(gameObject);
        }
    }
}
