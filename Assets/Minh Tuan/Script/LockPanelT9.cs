using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LockPanelT9 : MonoBehaviour
{
    [Header("Target password")]
    [SerializeField] private string targetPassword = "HI VONG"; // mật khẩu chỉ còn HI VONG

    [Header("UI")]
    [SerializeField] private TMP_Text displayText;   // hiển thị chuỗi nhập
    [SerializeField] private TMP_Text feedbackText;  // báo đúng/sai

    [Header("Buttons")]
    [SerializeField] private Button[] digitButtons;  // 9 nút chính
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button spaceButton;     // nút khoảng cách

    [Header("Events")]
    public UnityEvent onUnlock;
    public UnityEvent onWrongInput;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failClip;

    private float lastPressTime = 0f;
    private int lastButtonIndex = -1;

    private string[] keyMap = new string[]
    {
        "1ABC", "2DEF", "3GHI",
        "4JKL", "5MNO", "6PQR",
        "7STU", "8VWX", "9YZ0"
    };

    private string currentInput = "";
    private int[] pressIndex;

    private void Awake()
    {
        pressIndex = new int[digitButtons.Length];

        for (int i = 0; i < digitButtons.Length; i++)
        {
            int index = i;
            digitButtons[i].onClick.AddListener(() => HandleDigitPress(index));
        }

        if (deleteButton) deleteButton.onClick.AddListener(DeleteLast);
        if (confirmButton) confirmButton.onClick.AddListener(Confirm);
        if (spaceButton) spaceButton.onClick.AddListener(AddSpace);

        ResetInput();
    }

    private void HandleDigitPress(int index)
    {
        string chars = keyMap[index];

        if (index == lastButtonIndex && Time.time - lastPressTime < 1f)
        {
            pressIndex[index] = (pressIndex[index] + 1) % chars.Length;
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            currentInput += chars[pressIndex[index]];
        }
        else
        {
            pressIndex[index] = 0;
            currentInput += chars[pressIndex[index]];
        }

        lastPressTime = Time.time;
        lastButtonIndex = index;
        UpdateDisplay();
    }

    private void AddSpace()
    {
        currentInput += " ";
        UpdateDisplay();
    }

    private void DeleteLast()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    private void Confirm()
    {
        if (displayText.text == targetPassword)
        {
            feedbackText.text = "Đúng! Khóa đã mở.";
            if (audioSource && successClip) audioSource.PlayOneShot(successClip);
            onUnlock?.Invoke();
        }
        else
        {
            feedbackText.text = "Sai mật khẩu!";
            if (audioSource && failClip) audioSource.PlayOneShot(failClip);
            onWrongInput?.Invoke();
            ResetInput(false);
        }
    }

    private void UpdateDisplay()
    {
        if (displayText) displayText.text = currentInput;
    }

    private void ResetInput(bool clearFeedback = true)
    {
        currentInput = "";
        for (int i = 0; i < pressIndex.Length; i++) pressIndex[i] = -1;
        UpdateDisplay();
        if (clearFeedback && feedbackText) feedbackText.text = "";
    }
}