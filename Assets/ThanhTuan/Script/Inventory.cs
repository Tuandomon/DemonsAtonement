using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public int maxPotionCount = 5;
    public int currentPotionCount = 0;
    public int healAmountPerPotion = 30;

    private PlayerHealth playerHealth;

    public TMPro.TextMeshProUGUI potionCountText;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        UpdatePotionUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UsePotion();
        }
    }

    public bool AddPotion()
    {
        if (currentPotionCount < maxPotionCount)
        {
            currentPotionCount++;
            UpdatePotionUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UsePotion()
    {
        if (currentPotionCount > 0 && playerHealth != null)
        {
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                currentPotionCount--;
                playerHealth.AddHealth(healAmountPerPotion);
                UpdatePotionUI();
            }
        }
    }

    void UpdatePotionUI()
    {
        if (potionCountText != null)
        {
            potionCountText.text = currentPotionCount + " / " + maxPotionCount;
        }
    }
}