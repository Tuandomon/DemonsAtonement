using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingTrapLevel3 : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 25;

    [Header("Raycast Settings")]
    public Transform hitPoint;        // điểm ở đầu bẫy
    public float hitRange = 0.3f;     // khoảng kiểm tra
    public LayerMask playerLayer;

    private bool canDamage = false;

    // Gọi từ Animation Event khi đập xuống
    public void StartDamage()
    {
        canDamage = true;
        CheckDamage();
    }

    // Gọi từ Animation Event khi rút lên
    public void StopDamage()
    {
        canDamage = false;
    }

    private void CheckDamage()
    {
        if (!canDamage) return;

        // Raycast kiểm tra player ngay tại vị trí trap đập xuống
        Collider2D hit = Physics2D.OverlapCircle(hitPoint.position, hitRange, playerLayer);

        if (hit != null)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, hitRange);
    }
}


