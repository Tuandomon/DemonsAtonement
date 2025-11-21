using UnityEngine;

public class CoinandKeyManager : MonoBehaviour
{
    public static CoinandKeyManager instance;

    public int totalCoins = 0;
    public int totalKeys = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại khi đổi scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;
    }

    public void AddKey(int amount)
    {
        totalKeys += amount;
    }
}