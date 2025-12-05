using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private grab Grap;
    public Canvas GameOver;
    public TMP_Text TimerText;

    public float WinTime = 2f;       // Th?i gian ?? win
    public float ReturnDelay = 3f;    // 3 giây sau win tr? l?i map chính
    private bool isWin = false;

    private void Awake()
    {
        if (Grap != null)
        {
            Grap.PlayerDied += WhenPlayerDied;
        }

        if (GameOver.gameObject.activeSelf)
        {
            GameOver.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // WIN khi ?? 25 giây
        if (!isWin && Time.timeSinceLevelLoad >= WinTime)
        {
            WinMiniGame();
        }
    }

    // =============================
    //       X? LÝ THUA
    // =============================
    void WhenPlayerDied()
    {
        // Thua ? hi?n UI thua ? KHÔNG v? l?i scene chính
        GameOver.gameObject.SetActive(true);
        TimerText.text = "You lasted: " + Math.Round(Time.timeSinceLevelLoad, 2);

        if (Grap != null)
        {
            Grap.PlayerDied -= WhenPlayerDied;
        }
    }

    // =============================
    //       X? LÝ WIN
    // =============================
    void WinMiniGame()
    {
        isWin = true;

        // Thông báo th?ng
        GameOver.gameObject.SetActive(true);
        TimerText.text = "YOU WIN!";

        // Báo v? scene chính r?ng c?n r?t 2 item
        PlayerPrefs.SetInt("DropReward", 1);
        PlayerPrefs.Save();

        // 3 giây sau t? ??ng tr? v? scene chính
        Invoke(nameof(ReturnToMainScene), ReturnDelay);
    }

    // =============================
    //     TR? L?I SCENE CHÍNH
    // =============================
    void ReturnToMainScene()
    {
        SceneManager.LoadScene("Map2");
        // ? ??I "MainScene" thành tên scene chính c?a b?n
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
