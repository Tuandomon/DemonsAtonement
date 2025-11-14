using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool hasBeenActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenActivated)
        {
            hasBeenActivated = true;
            RespawnManager.Instance.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint mới đã được lưu: " + transform.position);
        }
        else if (hasBeenActivated)
        {
            Debug.Log("Checkpoint này đã được kích hoạt trước đó, không lưu lại.");
        }
    }
}