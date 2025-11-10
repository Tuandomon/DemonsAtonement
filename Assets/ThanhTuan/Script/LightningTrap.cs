using UnityEngine;

using System.Collections;



public class LightningTrap : MonoBehaviour

{

    // --- CÀI ĐẶT CHU KỲ & ANIMATION ---

    private Animator lightningAnimator;



    [Header("Chu kỳ & Animation")]

    public float safeDuration = 2.5f;   // Thời gian Cooldown (An toàn)

    public float activeDuration = 1.5f; // Thời gian giật sét (Nguy hiểm)

    private bool isShocking = false;    // Kiểm soát bẫy có đang gây sát thương không



    // --- CÀI ĐẶT SÁT THƯƠNG ---

    [Header("Cài đặt Sát thương")]

    public int damagePerSecond = 100; // Sát thương mỗi 1 giây

    public string targetTag = "Player";

    private float nextDamageTime; // Thời điểm gây sát thương tiếp theo



    // Tên Trigger cho Animator

    private readonly string ACTIVE_TRIGGER = "StartShock";





    void Awake()

    {

        lightningAnimator = GetComponent<Animator>();

    }



    void Start()

    {

        isShocking = false;

        StartCoroutine(ShockCycle());

    }



    IEnumerator ShockCycle()

    {

        while (true)

        {

            // GIAI ĐOẠN COOLDOWN

            isShocking = false;

            yield return new WaitForSeconds(safeDuration);



            // GIAI ĐOẠN NGUY HIỂM

            isShocking = true;

            lightningAnimator.SetTrigger(ACTIVE_TRIGGER); // Kích hoạt animation sét giật



            // Thiết lập thời điểm sát thương tiếp theo ngay khi bẫy bắt đầu giật

            nextDamageTime = Time.time;



            yield return new WaitForSeconds(activeDuration);

        }

    }



    // --- LOGIC SÁT THƯƠNG THEO THỜI GIAN (DoT) ---



    // Được gọi liên tục khi Collider đang chồng lấn (kể cả khi đứng yên)

    private void OnTriggerStay2D(Collider2D other)

    {

        // 1. Kiểm tra Tag

        // 2. Kiểm tra trạng thái bẫy (chỉ gây sát thương khi đang giật)

        if (other.CompareTag(targetTag) && isShocking)

        {

            // 3. Kiểm tra Thời gian (Đã đủ 1 giây chưa)

            if (Time.time >= nextDamageTime)

            {

                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();



                if (playerHealth != null)

                {

                    // Gây sát thương

                    playerHealth.TakeDamage(damagePerSecond);

                    Debug.Log($"Player bị sét giật! -{damagePerSecond} DMG.");

                }



                // Thiết lập thời điểm sát thương tiếp theo (sau 1 giây)

                nextDamageTime = Time.time + 1f;

            }

        }

    }



    // Khi Player chạm vào lần đầu, reset hẹn giờ để sát thương áp dụng ngay lập tức

    private void OnTriggerEnter2D(Collider2D other)

    {

        if (other.CompareTag(targetTag) && isShocking)

        {

            nextDamageTime = Time.time;

        }

    }

}