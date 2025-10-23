using UnityEngine;

public class SpearTrapDetector : MonoBehaviour
{
    private SpearTrap trapScript;

    void Start()
    {
        // Lấy script SpearTrap từ đối tượng cha
        trapScript = GetComponentInParent<SpearTrap>();

        if (trapScript == null)
        {
            Debug.LogError("SpearTrapDetector: Phải là con của GameObject có script SpearTrap!");
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra Player và trạng thái cooldown
        if (other.CompareTag(trapScript.targetTag) && !trapScript.isOnCooldown)
        {
            // Gọi Coroutine FireAndCooldown trên đối tượng cha
            trapScript.StartCoroutine("FireAndCooldown");
        }
    }
}