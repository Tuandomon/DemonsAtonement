using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int baseDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            int finalDamage = baseDamage;

            // Kiểm tra xem player có đang được buff không
            PlayerShooting player = GetComponentInParent<PlayerShooting>();
            if (player != null && player.isBuffed)
            {
                finalDamage = Mathf.RoundToInt(baseDamage * 1.5f); // tăng 50%
            }

            enemy.TakeDamage(finalDamage);
            Debug.Log($"Enemy trúng đòn từ {gameObject.name}, sát thương gây ra: {finalDamage}");
        }
    }
}