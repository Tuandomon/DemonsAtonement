using UnityEngine;

public class Summoner1 : MonoBehaviour
{
    [Header("Summon Settings")]
    public GameObject summonPrefab;
    public float summonCooldown = 15f;
    public float summonActivationRange = 7f; // Phạm vi kích hoạt skill triệu hồi

    [Header("Spawn Position")]
    public Transform summonPoint;

    private float nextSummonTime = 0f;
    private Transform player;
    private Animator animator;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (player == null || summonPoint == null || summonPrefab == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (Time.time >= nextSummonTime && distance <= summonActivationRange)
        {
            // Kiểm tra Boss có đang chạy không (tương tự như Boss1_ATK)
            if (animator != null && animator.GetBool("isRunning")) return;

            SummonMonster();
        }
    }

    private void SummonMonster()
    {
        // Kích hoạt animation triệu hồi (nếu bạn có trigger "summon")
        // if (animator != null) animator.SetTrigger("summon");

        // Triệu hồi quái vật
        Instantiate(summonPrefab, summonPoint.position, summonPoint.rotation);

        nextSummonTime = Time.time + summonCooldown;
    }
}