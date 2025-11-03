using UnityEngine;

public class FallingRockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public float respawnTime = 6f;
    public float randomOffset = 2f; // 🔹 Thêm: lệch ngẫu nhiên +/- 2 giây
    private bool isRespawning = false;

    void Start()
    {
        // Spawn lần đầu với chút ngẫu nhiên
        float firstDelay = Random.Range(0f, 1.5f);
        Invoke(nameof(SpawnRock), firstDelay);
    }

    public void StartRespawn()
    {
        if (!isRespawning)
        {
            isRespawning = true;
            float delay = respawnTime + Random.Range(-randomOffset, randomOffset);
            delay = Mathf.Max(1f, delay); // không cho âm
            Invoke(nameof(SpawnRock), delay);
        }
    }

    void SpawnRock()
    {
        GameObject rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
        var rockScript = rock.GetComponent<FallingRock>();
        if (rockScript != null)
            rockScript.SetSpawner(this);

        isRespawning = false;
    }
}
