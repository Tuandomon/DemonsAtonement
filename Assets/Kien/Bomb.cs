using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [Header("Bomb settings")]
    public float explosionRadius = 2.5f;
    public int damage = 50;
    public LayerMask damageLayerMask;
    public bool explodeOnContact = true;
    public float respawnTime = 5f;

    [Header("Effects & audio")]
    public GameObject explosionEffectPrefab; // Hiệu ứng cháy
    public AudioClip explosionSound;

    private bool hasExploded = false;
    private Collider2D col2d;
    private SpriteRenderer sr;
    private AudioSource audioSource;
    private GameObject activeEffect; // lưu hiệu ứng cháy đang hiện

    void Awake()
    {
        col2d = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (explodeOnContact && !hasExploded)
        {
            Explode();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (explodeOnContact && !hasExploded)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Tạo hiệu ứng cháy (lưu lại để sau này xóa)
        if (explosionEffectPrefab != null)
        {
            activeEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Âm thanh
        if (explosionSound != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(explosionSound);
            else
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Gây sát thương cho player
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayerMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<PlayerHealth>(out var ph))
                ph.TakeDamage(damage);
            else
                hit.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        // Ẩn bomb
        if (sr != null) sr.enabled = false;
        if (col2d != null) col2d.enabled = false;

        // Bắt đầu đếm hồi
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);

        // Xóa hiệu ứng cháy khi bomb hồi lại
        if (activeEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = null;
        }

        // Reset trạng thái bomb
        hasExploded = false;
        if (sr != null) sr.enabled = true;
        if (col2d != null) col2d.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
