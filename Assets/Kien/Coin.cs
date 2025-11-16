using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Collider2D col;
    public AudioClip pickSound;

    void Start()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;    // khóa ăn ngay khi xuất hiện
        StartCoroutine(EnablePickupAfterDelay());
    }

    IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(2f);  // đợi 2 giây
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && col.enabled)
        {
            // xử lý khi player nhặt chìa khóa
            Debug.Log("Player picked up the key!");

            if (pickSound != null)
                AudioSource.PlayClipAtPoint(pickSound, transform.position);

            Destroy(gameObject);
        }
    }
}
