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
        // Kiểm tra Button_instruct
        if (Button_instruct != null)
        {
            // Xóa listener cũ để tránh lỗi khi reload scene
            Button_instruct.onClick.RemoveAllListeners();
            Button_instruct.onClick.AddListener(TogglePanel);
        }
        else
        {
            Debug.LogWarning("PanelToggleWithVideo: Button_instruct chưa được gán!");
        }

        // Đảm bảo panel khởi tạo đúng trạng thái
        if (Panel_ins != null)
        {
            Panel_ins.SetActive(isPanelActive);
        }
        else
        {
            Debug.LogWarning("PanelToggleWithVideo: Panel_ins chưa được gán!");
        }

        // Kiểm tra VideoPlayer
        if (videoPlayer == null)
        {
            Debug.LogWarning("PanelToggleWithVideo: VideoPlayer chưa được gán!");
        }
    }

    void Update()
    {
        // Nhấn phím B cũng toggle
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
