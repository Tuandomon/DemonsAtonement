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
    {// L?y scene c?
        string prevScene = PlayerPrefs.GetString("Diem", "Map2");

        // L?ng nghe khi load ?? d?ch chuy?n Player
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

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
