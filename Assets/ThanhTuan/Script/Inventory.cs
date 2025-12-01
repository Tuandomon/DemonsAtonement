using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public int maxPotionCount = 5;
    public int currentPotionCount = 0;
    public int healAmountPerPotion = 1000;
    public TMPro.TextMeshProUGUI potionCountText;

    [Header("Buff Item Setup")]
    public int maxBuffItemCount = 3;
    public int currentBuffItemCount = 0;
    public TMPro.TextMeshProUGUI buffItemCountText;
    public GameObject buffEffectPrefab;
    public float buffDuration = 30f;

    [Header("Summon Item Setup")]
    public int maxSummonItemCount = 1;
    public int currentSummonItemCount = 0;
    public TMPro.TextMeshProUGUI summonItemCountText;
    public GameObject petPrefab;
    public float petSummonDuration = 30f;

    private PlayerHealth playerHealth;
    private PlayerShooting playerShooting;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerShooting = GetComponent<PlayerShooting>();
        UpdatePotionUI();
        UpdateBuffItemUI();
        UpdateSummonItemUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UsePotion();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            UseBuffItem();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseSummonItem();
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

    public bool AddBuffItem()
    {
        if (currentBuffItemCount < maxBuffItemCount)
        {
            currentBuffItemCount++;
            UpdateBuffItemUI();
            return true;
        }
        return false;
    }

    public void UseBuffItem()
    {
        if (playerShooting == null) return;
        if (playerShooting.isBuffActive) return;

        if (currentBuffItemCount > 0)
        {
            currentBuffItemCount--;
            if (buffEffectPrefab != null)
            {
                GameObject player = playerShooting.gameObject;
                GameObject effectInstance = Instantiate(buffEffectPrefab, player.transform.position, Quaternion.identity);
                effectInstance.transform.SetParent(player.transform);
                Destroy(effectInstance, buffDuration);
            }
            playerShooting.ActivateBuff(buffDuration, true);
            UpdateBuffItemUI();
        }
    }

    public bool AddSummonItem()
    {
        if (currentSummonItemCount < maxSummonItemCount)
        {
            currentSummonItemCount++;
            UpdateSummonItemUI();
            return true;
        }
        return false;
    }

    public void UseSummonItem()
    {
        if (currentSummonItemCount > 0)
        {
            if (petPrefab != null)
            {
                currentSummonItemCount--;
                GameObject pet = Instantiate(petPrefab, transform.position, Quaternion.identity);
                Destroy(pet, petSummonDuration);
                UpdateSummonItemUI();
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

    void UpdateBuffItemUI()
    {
        if (buffItemCountText != null)
        {
            buffItemCountText.text = currentBuffItemCount + " / " + maxBuffItemCount;
        }
    }

    void UpdateSummonItemUI()
    {
        if (summonItemCountText != null)
        {
            summonItemCountText.text = currentSummonItemCount + " / " + maxSummonItemCount;
        }
    }
}