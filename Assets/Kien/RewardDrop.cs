using UnityEngine;

public class RewardDrop : MonoBehaviour
{
    public GameObject itemPrefab1;   // Item th? 1
    public GameObject itemPrefab2;   // Item th? 2
    public Transform dropPoint;      // ?i?m r?t

    void Start()
    {
        int reward = PlayerPrefs.GetInt("DropReward", 0);

        if (reward == 1)
        {
            DropItems();
            PlayerPrefs.SetInt("DropReward", 0);
            PlayerPrefs.Save();
        }
    }

    void DropItems()
    {
        // R?t item 1
        Instantiate(itemPrefab1, dropPoint.position + new Vector3(-0.5f, 0, 0), Quaternion.identity);

        // R?t item 2
        Instantiate(itemPrefab2, dropPoint.position + new Vector3(0.5f, 0, 0), Quaternion.identity);
    }
}
