using UnityEngine;

public class RewardDrop1 : MonoBehaviour
{
    public GameObject itemPrefab3;   // Item th? 1
    public GameObject itemPrefab4;   // Item th? 2
    public Transform dropPoints;      // ?i?m r?t

    void Start()
    {
        int reward = PlayerPrefs.GetInt("DropReward1", 0);

        if (reward == 1)
        {
            DropItems();
            PlayerPrefs.SetInt("DropReward1", 0);
            PlayerPrefs.Save();
        }
    }

    void DropItems()
    {
        // R?t item 1
        Instantiate(itemPrefab3, dropPoints.position + new Vector3(-0.5f, 0, 0), Quaternion.identity);

        // R?t item 2
        Instantiate(itemPrefab4, dropPoints.position + new Vector3(0.5f, 0, 0), Quaternion.identity);
    }
}
