using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 30;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} bị trúng đạn! Máu còn: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} đã chết!");
        Destroy(gameObject);
    }
}
