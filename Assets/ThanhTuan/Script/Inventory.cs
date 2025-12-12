using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip useSuccessClip; // Âm thanh sử dụng vật phẩm thành công
    public AudioClip useFailureClip; // Âm thanh sử dụng vật phẩm thất bại (không có, đang buff, full máu)

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

        // THÊM: Gán AudioSource nếu chưa có
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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

    // ... Các hàm AddItem (không đổi) ...

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
        bool success = false;

        if (currentPotionCount > 0 && playerHealth != null)
        {
            // Kiểm tra: Có thể hồi máu (chưa full máu)
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                currentPotionCount--;
                playerHealth.AddHealth(healAmountPerPotion);
                UpdatePotionUI();
                success = true; // Thành công
            }
        }

        // THÊM: Phát âm thanh
        PlaySound(success);
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
        bool success = false;

        if (playerShooting == null)
        {
            PlaySound(false);
            return;
        }

        // Kiểm tra: Đã có buff đang hoạt động?
        if (playerShooting.isBuffActive)
        {
            PlaySound(false);
            return;
        }

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
            success = true; // Thành công
        }

        // THÊM: Phát âm thanh
        PlaySound(success);
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
        bool success = false;

        if (currentSummonItemCount > 0)
        {
            if (petPrefab != null)
            {
                currentSummonItemCount--;
                GameObject pet = Instantiate(petPrefab, transform.position, Quaternion.identity);
                Destroy(pet, petSummonDuration);
                UpdateSummonItemUI();
                success = true; // Thành công
            }
        }

        // THÊM: Phát âm thanh
        PlaySound(success);
    }

    // THÊM: Hàm tiện ích để phát âm thanh
    private void PlaySound(bool success)
    {
        if (audioSource == null) return;

        if (success)
        {
            if (useSuccessClip != null)
            {
                audioSource.PlayOneShot(useSuccessClip);
            }
        }
        else
        {
            if (useFailureClip != null)
            {
                audioSource.PlayOneShot(useFailureClip);
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