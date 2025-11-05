using UnityEngine;

public class ChestOpenOnce : MonoBehaviour
{
    public GameObject buttonUI;
    public Transform player;
    public Animator chestAnimator;

    [Header("Coin Settings")]
    public GameObject coinPrefab;        // prefab đồng xu
    public int coinCount = 5;            // số xu bắn ra
    public float spawnForce = 4f;        // lực bắn xu ra xung quanh
    public Transform spawnPoint;         // điểm xuất phát (ví dụ miệng rương)

    private bool isPlayerNear = false;
    private bool isOpened = false;

    void Start()
    {
        buttonUI.SetActive(false);
        chestAnimator.enabled = false;
    }

    void Update()
    {
        if (isPlayerNear && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            chestAnimator.enabled = true;
            chestAnimator.Play("Chest_wood");
            isOpened = true;
            buttonUI.SetActive(false);

            SpawnCoins(); // 💰 Gọi hàm tạo xu
        }
    }

    void SpawnCoins()
    {
        if (coinPrefab == null) return;

        for (int i = 0; i < coinCount; i++)
        {
            // tạo xu tại vị trí spawnPoint hoặc vị trí rương
            Vector3 spawnPos = (spawnPoint != null ? spawnPoint.position : transform.position);

            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            // thêm lực ngẫu nhiên để xu bay tung ra
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // hướng bắn ngẫu nhiên
                Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.8f, 1.2f)).normalized;
                rb.AddForce(randomDir * spawnForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            isPlayerNear = true;
            buttonUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            buttonUI.SetActive(false);
        }
    }

    public bool IsOpened()
    {
        return isOpened;
    }
}
