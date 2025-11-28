using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private grab Grap;
    public Canvas GameOver;
    public TMP_Text TimerText;
    private void Awake()
    {
        if(Grap != null)
        {
            Grap.PlayerDied += WhenPlayerDied;
        }

        if (GameOver.gameObject.activeSelf)
        {
            GameOver.gameObject.SetActive(false);
        }
    }

    void WhenPlayerDied() {
        GameOver.gameObject.SetActive(true);
        TimerText.text = "You lasted: " + Math.Round(Time.timeSinceLevelLoad,2);

        if (Grap != null)
        {
            Grap.PlayerDied -= WhenPlayerDied;
        }

    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
