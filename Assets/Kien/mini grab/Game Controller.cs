using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private grab Grap;


    public Canvas GameOver;
    public TMP_Text TimerText;

    public float WinTime = 2f;
    public float ReturnDelay = 3f;

    private bool isWin = false;

    private void Awake()
    {
        if (Grap != null)
            Grap.PlayerDied += WhenPlayerDied;

        GameOver.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isWin && Time.timeSinceLevelLoad >= WinTime)
        {
            WinMiniGame();
        }
    }

    void WhenPlayerDied()
    {
        GameOver.gameObject.SetActive(true);
        TimerText.text = "You lasted: " + Math.Round(Time.timeSinceLevelLoad, 2);

        if (Grap != null)
            Grap.PlayerDied -= WhenPlayerDied;
    }

    // =============================
    // WIN
    // =============================
    void WinMiniGame()
    {
        isWin = true;

        GameOver.gameObject.SetActive(true);
        TimerText.text = "YOU WIN!";

        // Thưởng item
        PlayerPrefs.SetInt("DropReward", 1);
        PlayerPrefs.Save();

        // Lưu tên điểm quay lại
        PlayerPrefs.SetString("ReturnPointName", "WinReturnPoint");
        PlayerPrefs.Save();
        // MỞ VÙNG CHẶN
        PlayerPrefs.SetInt("LockZone1", 0);

        Invoke(nameof(ReturnToMainScene), ReturnDelay);
    }

    void ReturnToMainScene()
    {
        // Lấy scene cũ
        string prevScene = PlayerPrefs.GetString("Diem", "Map2 1");

        // Lắng nghe khi load để dịch chuyển Player
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(prevScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float x = PlayerPrefs.GetFloat("CheckpointX", player.transform.position.x);
            float y = PlayerPrefs.GetFloat("CheckpointY", player.transform.position.y);

            player.transform.position = new Vector2(x, y);
        }
    }
    // =============================
    // UI NÚT X – CỐ THOÁT
    // =============================
    public void ExitByXButton()
    {
        // Cố thoát → không win → không item → vẫn bị chặn
        PlayerPrefs.SetInt("DropReward", 0);
        PlayerPrefs.SetInt("MiniGameWin", 0);
        PlayerPrefs.Save();

        // Lưu tên điểm quay lại
        PlayerPrefs.SetString("ReturnPointName", "WinReturnPoint");
        PlayerPrefs.Save();
        // BỊ CHẶN
        PlayerPrefs.SetInt("LockZone1", 1);

        Invoke(nameof(ReturnToMainScene), ReturnDelay);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
