using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Image winerImage;      // ?nh hi?n th? khi win
    public Sprite playerSprite;   // ?nh khi X th?ng
    public Sprite enemySprite;    // ?nh khi O th?ng
    public Button retry;

    private void Awake()
    {
        retry.onClick.AddListener(OnClick);
    }

    public void SetName(string s)
    {
        // N?u X th?ng ? hi?n hình Player
        if (s == "x")
        {
            winerImage.sprite = playerSprite;
            StartCoroutine(GoToNextScene());
        }
        else if (s == "0")
        {
            winerImage.sprite = enemySprite;
        }
    }

    IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(3f);

        // L?u tr?ng thái ?? map chính bi?t r?t item
        PlayerPrefs.SetInt("DropReward1", 1);

        string sceneName = PlayerPrefs.GetString("Diem", "Map2");

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
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

    public void OnClick()
    {
        SceneManager.LoadScene("Caro");
    }
}
