using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Gọi hàm nhận sát thương bên enemy
            other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        }
    }
}
