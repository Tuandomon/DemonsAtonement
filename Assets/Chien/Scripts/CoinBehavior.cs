using UnityEngine;
using System.Collections;

public class CoinBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform player;
    private bool isFlyingToPlayer = false;
    private float flySpeed = 8f; // tốc độ bay về player
    private float pickUpDistance = 0.5f; // khoảng cách để "nhặt" xu

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ✅ Bỏ va chạm vật lý giữa Player và Coin
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        Collider2D coinCollider = GetComponent<Collider2D>();
        if (playerCollider && coinCollider)
            Physics2D.IgnoreCollision(playerCollider, coinCollider, true);
    }

    private void Update()
    {
        if (isFlyingToPlayer && player != null)
        {
            // Bay về player
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * flySpeed * Time.deltaTime);

            // Khi tới gần player thì nhặt xu
            if (Vector2.Distance(transform.position, player.position) < pickUpDistance)
            {
                Debug.Log("Nhặt xu!");
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Khi xu chạm đất
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.isKinematic = true; // Ngừng rơi
            rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().isTrigger = true; // đổi sang trigger
            StartCoroutine(WaitAndFlyToPlayer()); // ⏳ đợi 3 giây rồi bay
        }
    }

    private IEnumerator WaitAndFlyToPlayer()
    {
        yield return new WaitForSeconds(1f); // ⏰ chờ 3 giây
        isFlyingToPlayer = true;
    }
}
