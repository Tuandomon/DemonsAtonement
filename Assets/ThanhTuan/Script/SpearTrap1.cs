using UnityEngine;
using System.Collections; // Cần thiết cho Coroutines

public class SpearTrap1 : MonoBehaviour
{
    // Tham số thiết lập trong Inspector
    [Header("Damage Settings")]
    public float dotDamagePerSecond = 100f; // Sát thương DOT
    public int thrustDamage = 20;            // Sát thương Burst (một lần)

    [Header("Timing Settings")]
    public float thrustDuration = 2f;        // Thời gian giáo đâm lên (Spear_11)
    public float minRetractTime = 5f;
    public float maxRetractTime = 10f;

    // Thành phần cần thiết
    private Animator anim;
    private BoxCollider2D triggerCollider;
    private bool isPlayerInsideTrigger = false;
    private bool isThrusting = false;

    // Biến trạng thái
    private enum TrapState { AnchorIdle, Thrusting, Retracting }
    private TrapState currentState = TrapState.AnchorIdle;

    private void Start()
    {
        anim = GetComponent<Animator>();
        triggerCollider = GetComponent<BoxCollider2D>();
        // Bắt đầu chu trình bẫy
        StartCoroutine(TrapCycle());
    }

    private void Update()
    {
        // Gây sát thương DOT khi người chơi ở trong trigger và giáo đang ở trạng thái neo
        if (isPlayerInsideTrigger && currentState == TrapState.AnchorIdle)
        {
            // Thay đổi PlayerHealth (Giả định) bằng giá trị sát thương DOT
            // Giả sử có một hàm TakeDamage(float damage) trên script của Player
            // FindObjectOfType<PlayerHealth>().TakeDamage(dotDamagePerSecond * Time.deltaTime);
            Debug.Log($"DOT Damage: -{dotDamagePerSecond * Time.deltaTime:F2}");
        }
    }

    // Xử lý khi người chơi bước vào/rời khỏi vùng trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
        }
    }
    // Coroutine chính quản lý chu trình của bẫy
    private IEnumerator TrapCycle()
    {
        // Lặp vô hạn để bẫy hoạt động liên tục
        while (true)
        {
            // **Trạng thái 1: Neo (AnchorIdle)**
            currentState = TrapState.AnchorIdle;
            Debug.Log("Spear Trap: Anchor Idle. DOT Active.");

            // Chờ người chơi lọt vào vùng trigger và thời gian chờ ngẫu nhiên
            float retractTime = Random.Range(minRetractTime, maxRetractTime);
            float timer = 0f;

            // Đợi cho đến khi người chơi vào vùng hoặc timer hết
            while (!isPlayerInsideTrigger && timer < retractTime)
            {
                timer += Time.deltaTime;
                yield return null; // Chờ 1 frame
            }

            // Nếu người chơi ở trong vùng hoặc timer hết (bẫy tự động kích hoạt)
            if (isPlayerInsideTrigger || timer >= retractTime)
            {
                yield return StartCoroutine(ThrustSequence()); // Bắt đầu đâm
            }

            // **Trạng thái 3: Rút về (Retracting)**
            currentState = TrapState.Retracting;
            Debug.Log("Spear Trap: Retracting. Cooldown.");

            // Thời gian chờ ngẫu nhiên giữa các lần đâm
            float cooldown = Random.Range(minRetractTime, maxRetractTime);
            yield return new WaitForSeconds(cooldown);
        }
    }

    // Coroutine xử lý quá trình giáo đâm
    private IEnumerator ThrustSequence()
    {
        currentState = TrapState.Thrusting;
        Debug.Log("Spear Trap: Thrusting initiated.");

        // Kích hoạt animation đâm lên (Spear_1 -> Spear_11)
        anim.SetTrigger("Thrust");

        // Đợi một khoảng thời gian ngắn để đảm bảo animation đạt frame sát thương (Spear_11)
        yield return new WaitForSeconds(0.5f); // Điều chỉnh tùy theo tốc độ animation

        // **Gây sát thương Burst (chỉ 1 lần)**
        if (isPlayerInsideTrigger)
        {
            // Giả sử có một hàm TakeDamage(int damage) trên script của Player
            // FindObjectOfType<PlayerHealth>().TakeDamage(thrustDamage);
            Debug.Log($"Burst Damage: -{thrustDamage} (One-time hit)");
        }

        // Đợi hết thời gian giáo đâm lên (Spear_11)
        yield return new WaitForSeconds(thrustDuration);

        // **Giáo rút về và reset trạng thái**
        // Tùy vào cách bạn thiết lập Animator, bạn có thể cần set một trigger/bool để rút về
        // Nếu không, hãy đảm bảo animation rút về tự động phát sau animation đâm lên

        // Đợi cho đến khi animation rút về hoàn tất (tùy thuộc vào tốc độ animation)
        // Ví dụ: Đợi 1 giây để rút về
        yield return new WaitForSeconds(1f);
    }
}
