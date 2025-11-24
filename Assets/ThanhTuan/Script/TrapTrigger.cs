using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private GameObject sandwormTrapPrefab;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform spawnPoint;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 5f;
    private float nextActivationTime = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra Player và thời gian hồi chiêu
        if (other.CompareTag(playerTag) && Time.time >= nextActivationTime)
        {
            // Thiết lập thời điểm kích hoạt tiếp theo
            nextActivationTime = Time.time + cooldownDuration;

            // Tạm ẩn Cát Phải (Đối tượng này) để nhường chỗ cho Cát Trái hoạt động
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<Collider2D>().enabled = false;

            // Tạo Prefab Sandworm (Cát Trái)
            GameObject newTrap = Instantiate(sandwormTrapPrefab, spawnPoint.position, spawnPoint.rotation);
            SandwormTrap trapScript = newTrap.GetComponent<SandwormTrap>();

            if (trapScript != null)
            {
                // Gắn tham chiếu của Cát Phải vào script bẫy
                trapScript.triggerObject = this.gameObject;
                trapScript.ActivateTrap(other.gameObject);
            }
        }
    }
}