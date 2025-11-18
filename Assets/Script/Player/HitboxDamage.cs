using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int baseDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            int finalDamage = baseDamage;

            PlayerShooting playerShoot = GetComponentInParent<PlayerShooting>();
            if (playerShoot != null && playerShoot.isBuffed)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * 1.5f);
            }

            enemy.TakeDamage(finalDamage);
            Debug.Log($"Enemy trúng đòn từ {gameObject.name}, sát thương gây ra: {finalDamage}");
        }
    }
}