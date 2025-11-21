using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThangNay : MonoBehaviour
{
    public Transform startPoint;      // v? trí ban ??u
    public Transform endPoint;        // v? trí ?ích
    public float speed = 2f;          // t?c ?? di chuy?n

    private bool playerOnPlatform = false;

    private void Update()
    {
        if (playerOnPlatform)
        {
            // Player ??ng ? thang máy ?i lên
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPoint.position,
                speed * Time.deltaTime
            );
        }
        else
        {
            // Player r?i ? thang máy tr? v? ch? c?
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPoint.position,
                speed * Time.deltaTime
            );
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = true;

            // Cho player ?i cùng platform
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = false;

            // Player ra ? b? parent
            collision.collider.transform.SetParent(null);
        }
    }
}
