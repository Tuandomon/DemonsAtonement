using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Vector3 currentCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
        Debug.Log("Checkpoint đã lưu: " + currentCheckpoint);
    }

    public void Respawn(GameObject player)
    {
        if (currentCheckpoint != Vector3.zero)
        {
            player.transform.position = currentCheckpoint;
            Debug.Log("Player đã hồi sinh tại checkpoint: " + currentCheckpoint);
        }
        else
        {
            Debug.LogWarning("Chưa có checkpoint nào được lưu!");
        }
    }
}