using UnityEngine;

public class TreeHint : MonoBehaviour
{
    [Header("Spawn Item")]
    public GameObject hintPrefab;
    public Transform spawnPoint;

    [Header("Shake Settings")]
    public float shakeAmount = 0.1f;
    public float shakeDuration = 0.15f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Leaf Effect")]
    public GameObject leafEffectPrefab;
    public Transform[] leafSpawnPoints;   // ⭐ 3 điểm spawn lá
    public int leafCountPerClick = 3;     // mỗi điểm spawn ra bao nhiêu lá

    private int clickCount = 0;
    private bool isShaking = false;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        if (isShaking) return;

        PlayClickSound();
        SpawnLeaves(); // 🌿 spawn nhiều lá từ 3 điểm

        clickCount++;

        if (clickCount == 1)
        {
            StartCoroutine(ShakeTree());
        }
        else if (clickCount == 2)
        {
            StartCoroutine(ShakeAndDestroy());
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    private void SpawnLeaves()
    {
        if (leafEffectPrefab == null || leafSpawnPoints.Length == 0)
            return;

        foreach (Transform point in leafSpawnPoints)
        {
            if (point == null) continue;

            // Spawn đúng 1 chiếc lá
            Instantiate(leafEffectPrefab, point.position, Quaternion.identity);
        }
    }



    private System.Collections.IEnumerator ShakeAndDestroy()
    {
        yield return StartCoroutine(ShakeTree());
        SpawnHintItem();
        Destroy(gameObject);
    }

    private void SpawnHintItem()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        if (hintPrefab != null)
            Instantiate(hintPrefab, pos, Quaternion.identity);
    }

    private System.Collections.IEnumerator ShakeTree()
    {
        isShaking = true;

        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return null;
        }

        transform.localPosition = originalPos;
        isShaking = false;
    }
}
