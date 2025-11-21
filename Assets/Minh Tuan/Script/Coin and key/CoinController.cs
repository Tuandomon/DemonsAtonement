using UnityEngine;
using TMPro;

public class CoinController : MonoBehaviour
{
    private TextMeshProUGUI coinText;
    private int coinCount = 0;

    private void Start()
    {
        // Tự động tìm TextMeshPro có tên "TextCoin" trong scene
        GameObject obj = GameObject.Find("TextCoin");
        if (obj != null)
        {
            coinText = obj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Không tìm thấy TextCoin trong Canvas!");
        }

        // Nếu đã có dữ liệu từ CoinandKeyManager thì hiển thị lại
        coinCount = CoinandKeyManager.instance.totalCoins;
        UpdateCoinUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            coinCount++;
            CoinandKeyManager.instance.AddCoin(1);
            UpdateCoinUI();

            Destroy(collision.gameObject);
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coinCount.ToString();
        }
    }
}