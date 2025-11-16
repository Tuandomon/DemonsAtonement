using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuaCong : MonoBehaviour
{
    [Header("Scene & Spawn Settings")]
    public string targetSceneName = "Map2";      // Scene đích
    public string targetSpawnName = "Door2_Spawn"; // Tên object spawn trong scene đích

    [Header("Phím tương tác")]
    public KeyCode interactKey = KeyCode.E;

    bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            // (tuỳ chọn) hiển thị UI "Nhấn E để vào cổng"
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            // (tuỳ chọn) ẩn UI
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            // Lưu tên spawn point trước khi đổi scene
            teleData.spawnPointName = targetSpawnName;
            teleData.useSpawnPoint = true;

            // Chuyển scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
