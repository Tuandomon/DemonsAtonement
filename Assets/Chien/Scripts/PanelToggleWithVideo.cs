using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PanelToggleWithVideo : MonoBehaviour
{
    [Header("UI & Video References")]
    public GameObject Panel_ins;          // Panel muốn bật/tắt
    public Button Button_instruct;        // Nút click để toggle
    public VideoPlayer videoPlayer;       // VideoPlayer gắn trên panel

    private bool isPanelActive = false;

    void Start()
    {
        if (Button_instruct != null)
        {
            Button_instruct.onClick.AddListener(TogglePanel);
        }

        if (Panel_ins != null)
        {
            Panel_ins.SetActive(isPanelActive);
        }
    }

    void Update()
    {
        // Nhấn phím L cũng toggle
        if (Input.GetKeyDown(KeyCode.B))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        isPanelActive = !isPanelActive;

        if (Panel_ins != null)
        {
            Panel_ins.SetActive(isPanelActive);
        }

        if (videoPlayer != null)
        {
            if (isPanelActive)
            {
                // Khi bật panel, phát lại video từ đầu
                videoPlayer.time = 0;
                videoPlayer.Play();
            }
            else
            {
                // Khi tắt panel, dừng video và reset về đầu
                videoPlayer.Stop();
                videoPlayer.time = 0;
            }
        }
    }
}
