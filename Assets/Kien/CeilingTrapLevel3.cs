using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingTrapLevel3 : MonoBehaviour
{
    [Header("Thiết lập Trap")]
    public float dropSpeed = 15f;           // tốc độ rơi
    public float returnSpeed = 5f;          // tốc độ nâng lại
    public float dropDelay = 1.5f;          // thời gian chờ trước khi rơi
    public float resetDelay = 1f;           // thời gian chờ trước khi nâng lên

    [Header("Damage")]
    public int damage = 30;
    public float stunDuration = 0.5f;

    [Header("Âm thanh")]
    public AudioClip dropSound;
    public AudioClip hitGroundSound;

    [Header("Điểm")]
    public Transform startPos;     // vị trí ban đầu (trên)
    public Transform endPos;       // vị trí đập xuống (dưới)

    private bool isDropping = false;

    void Start()
    {
        transform.position = startPos.position;
        StartCoroutine(TrapRoutine());
    }

    IEnumerator TrapRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dropDelay);

            // 🔻 Rơi xuống
            isDropping = true;
            if (dropSound) AudioSource.PlayClipAtPoint(dropSound, transform.position);

            while (Vector2.Distance(transform.position, endPos.position) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    endPos.position,
                    dropSpeed * Time.deltaTime
                );
                yield return null;
            }

            isDropping = false;

            // ⏳ chạm đất
            if (hitGroundSound) AudioSource.PlayClipAtPoint(hitGroundSound, transform.position);
            yield return new WaitForSeconds(resetDelay);

            // 🔼 Nâng trap lên lại
            while (Vector2.Distance(transform.position, startPos.position) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    startPos.position,
                    returnSpeed * Time.deltaTime
                );
                yield return null;
            }
        }
    }

    // 🎯 Gây damage khi đập trúng Player
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDropping && collision.CompareTag("Player"))
        {
            PlayerHealth hp = collision.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

        }
    }
}


