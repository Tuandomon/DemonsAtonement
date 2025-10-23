using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingPanel;

    void Start()
    {
        if (settingPanel != null)
            settingPanel.SetActive(false); // Ẩn panel Setting lúc đầu

        if (pausePanel != null)
            pausePanel.SetActive(true); // Hiện panel Pause lúc đầu
    }

    public void OpenSettings()
    {
        Time.timeScale = 0;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingPanel != null)
            settingPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        if (settingPanel != null)
            settingPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }
}