using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCva : MonoBehaviour
{

    public float speed = 2f;         // tốc độ di chuyển
    public float moveRange = 3f;     // khoảng cách tối đa đi qua lại (tính từ vị trí ban đầu)
    public int health = 20;          // máu NPC

    private float leftLimit, rightLimit;
    private bool movingRight = true;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        leftLimit = transform.position.x - moveRange;
        rightLimit = transform.position.x + moveRange;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= rightLimit)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= leftLimit)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    void Flip()
    {
        if (sr != null)
            sr.flipX = !sr.flipX;
        else
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi bị vũ khí của người chơi va chạm
        if (other.CompareTag("PlayerWeapon"))
        {
            TakeDamage(20); // bị chém 1 lần chết luôn
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
