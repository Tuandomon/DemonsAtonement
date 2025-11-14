using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trơn : MonoBehaviour
{
    public PhysicsMaterial2D normalMaterial;   // Material ma sát bình thường
    public PhysicsMaterial2D slipperyMaterial; // Material trơn
    public float minMoveSpeed = 0.1f;          // Tốc độ tối thiểu để tính là "đang di chuyển"

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Collider2D col = GetComponent<Collider2D>();

            if (Mathf.Abs(rb.velocity.x) > minMoveSpeed)
            {
                // Player đang di chuyển → trơn
                col.sharedMaterial = slipperyMaterial;
            }
            else
            {
                // Player đứng yên → không trượt
                col.sharedMaterial = normalMaterial;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reset lại ma sát khi player rời đi
            Collider2D col = GetComponent<Collider2D>();
            col.sharedMaterial = normalMaterial;
        }
    }
}
