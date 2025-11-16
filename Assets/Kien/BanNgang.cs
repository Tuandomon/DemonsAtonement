using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanNgang : MonoBehaviour
{
    [Header("Thiết lập bắn")]
    public GameObject spikePrefab;
    public Transform shootPoint;
    public float shootInterval = 1.5f;
    public float spikeSpeed = 10f;

    [Header("Detection Box (nằm ngang)")]
    public Vector2 boxSize = new Vector2(6f, 2f); // rộng - cao
    public LayerMask detectionMask;

    Transform targetPlayer;

    void Start()
    {
        StartCoroutine(ShootingLoop());
    }

    IEnumerator ShootingLoop()
    {
        while (true)
        {
            if (DetectPlayer())
            {
                ShootTowardPlayerHorizontal();
            }

            yield return new WaitForSeconds(shootInterval);
        }
    }

    bool DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapBox(
            shootPoint.position,
            boxSize,
            0f,
            detectionMask
        );

        if (hit != null)
        {
            targetPlayer = hit.transform;
            return true;
        }

        targetPlayer = null;
        return false;
    }

    void ShootTowardPlayerHorizontal()
    {
        if (targetPlayer == null) return;

        // Xác định hướng trái/phải dựa vào vị trí Player
        Vector2 dir;

        if (targetPlayer.position.x > shootPoint.position.x)
            dir = Vector2.right;    // Player bên phải → bắn phải
        else
            dir = Vector2.left;     // Player bên trái → bắn trái

        // tạo đạn
        GameObject s = Instantiate(spikePrefab, shootPoint.position, Quaternion.identity);

        // đặt vận tốc
        Rigidbody2D rb = s.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dir * spikeSpeed;

        // xoay sprite cho đúng hướng
        if (dir == Vector2.right)
            s.transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            s.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    // Vẽ vùng detection box
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(shootPoint.position, boxSize);
    }
}
