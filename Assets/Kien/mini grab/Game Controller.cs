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
    //              WIN
    // =============================
    void WinMiniGame()
    {
        isWin = true;

        GameOver.gameObject.SetActive(true);
        TimerText.text = "YOU WIN!";

        // Th??ng item
        PlayerPrefs.SetInt("DropReward", 1);
        PlayerPrefs.Save();

        // L?U TÊN ?I?M QUAY L?I
        PlayerPrefs.SetString("ReturnPointName", "WinReturnPoint");
        PlayerPrefs.Save();

        Invoke(nameof(ReturnToMainScene), ReturnDelay);
    }

    void ReturnToMainScene()
    {
        SceneManager.LoadScene("Map2");
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
