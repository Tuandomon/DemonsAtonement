using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int baseDamage = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            int finalDamage = baseDamage;

            // Kiểm tra trạng thái buff từ PlayerShooting
            PlayerShooting player = GetComponentInParent<PlayerShooting>();
            if (player != null && player.isBuffed)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * 1.5f); // tăng 50%
            }

            // Gọi hàm nhận sát thương bên enemy
            other.GetComponent<EnemyHealth>()?.TakeDamage(finalDamage);

            Debug.Log($"Enemy trúng đòn từ {gameObject.name}, sát thương gây ra: {finalDamage}");
        }
    }
}
