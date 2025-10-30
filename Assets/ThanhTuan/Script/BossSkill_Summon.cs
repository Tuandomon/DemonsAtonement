using UnityEngine;
using System.Collections;

public class BossSkill_Summon : MonoBehaviour
{
    [Header("Summon Settings")]
    public GameObject minionPrefab; // Kéo thả prefab của sinh vật/minion vào đây
    public int numberOfMinions = 3; // Số lượng minion muốn triệu hồi
    public float summonRadius = 5f; // Bán kính xung quanh Boss để triệu hồi minion
    public float skillCooldown = 15f; // Thời gian hồi chiêu
    private bool canUseSkill = true;

    // Tùy chọn: Bạn có thể muốn Boss chỉ dùng skill này khi máu thấp hoặc theo giai đoạn
    public BossHealth bossHealth;

    void Update()
    {
        // Ví dụ: Boss tự động dùng skill khi sẵn sàng
        if (canUseSkill)
        {
            StartCoroutine(UseSummonSkill());
        }
    }

    IEnumerator UseSummonSkill()
    {
        canUseSkill = false;

        Debug.Log("Boss bắt đầu triệu hồi sinh vật...");

        for (int i = 0; i < numberOfMinions; i++)
        {
            // 1. Tính toán vị trí ngẫu nhiên xung quanh Boss
            Vector3 randomPos = Random.insideUnitCircle * summonRadius;
            Vector3 summonPosition = transform.position + new Vector3(randomPos.x, transform.position.y, randomPos.y);
            // Giả định game 3D (X, Z là mặt phẳng). Nếu game 2D (X, Y) thì dùng Vector2.

            // 2. Triệu hồi Minion
            Instantiate(minionPrefab, summonPosition, Quaternion.identity);
        }

        Debug.Log("Đã triệu hồi " + numberOfMinions + " sinh vật.");

        // 3. Chờ hồi chiêu
        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }
}