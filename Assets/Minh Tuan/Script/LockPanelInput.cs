using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LockPanelInput : MonoBehaviour
{
    [Header("Target password")]
    [SerializeField] private string targetLetters = "HIVONG";
    [SerializeField] private int[] targetNumbers = new int[] { 8, 7, 1 };

    [Header("UI Slots")]
    [SerializeField] private TMP_Text[] letterSlots; // 6 ô chữ
    [SerializeField] private TMP_Text[] numberSlots; // 3 ô số
    [SerializeField] private TMP_Text feedbackText;

    [Header("Buttons")]
    [SerializeField] private Button[] letterButtons; // 6 nút chữ
    [SerializeField] private Button[] numberButtons; // 3 nút số
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button confirmButton;

    [Header("Events")]
    public UnityEvent onUnlock;
    public UnityEvent onWrongInput;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // AudioSource chung
    [SerializeField] private AudioClip successClip;     // âm thanh đúng mật khẩu
    [SerializeField] private AudioClip failClip;        // âm thanh sai mật khẩu

    private int currentLetterIndex = 0;
    private int currentNumberIndex = 0;

    private void Awake()
    {
        // Gán sự kiện cho nút chữ
        for (int i = 0; i < letterButtons.Length; i++)
        {
            int index = i;
            letterButtons[i].onClick.AddListener(() => AddLetter(targetLetters[index]));
        }

        // Gán sự kiện cho nút số
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int index = i;
            numberButtons[i].onClick.AddListener(() => IncreaseNumber(index));
        }

        // Nút xóa
        if (deleteButton) deleteButton.onClick.AddListener(DeleteLast);

        // Nút xác nhận
        if (confirmButton) confirmButton.onClick.AddListener(Confirm);

        ResetSlots();
    }

    private void AddLetter(char c)
    {
        if (currentLetterIndex < letterSlots.Length)
        {
            letterSlots[currentLetterIndex].text = c.ToString();
            currentLetterIndex++;
        }
    }

    private void IncreaseNumber(int index)
    {
        if (index < numberSlots.Length)
        {
            int current = string.IsNullOrEmpty(numberSlots[index].text) ? 0 : int.Parse(numberSlots[index].text);
            current = (current + 1) % 10;
            numberSlots[index].text = current.ToString();
            currentNumberIndex = Mathf.Max(currentNumberIndex, index + 1);
        }
    }

    private void DeleteLast()
    {
        if (currentNumberIndex > 0)
        {
            currentNumberIndex--;
            numberSlots[currentNumberIndex].text = "";
        }
        else if (currentLetterIndex > 0)
        {
            currentLetterIndex--;
            letterSlots[currentLetterIndex].text = "";
        }
    }

    private void Confirm()
    {
        string currentLetters = "";
        for (int i = 0; i < letterSlots.Length; i++)
        {
            currentLetters += letterSlots[i].text;
        }

        int[] currentNumbers = new int[numberSlots.Length];
        for (int i = 0; i < numberSlots.Length; i++)
        {
            if (string.IsNullOrEmpty(numberSlots[i].text)) { feedbackText.text = "Chưa nhập đủ số!"; return; }
            currentNumbers[i] = int.Parse(numberSlots[i].text);
        }

        bool lettersOk = currentLetters == targetLetters;
        bool numbersOk = AreNumbersEqual(currentNumbers, targetNumbers);

        if (lettersOk && numbersOk)
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
            ShuffleLetters(); // đổi vị trí nút chữ khi sai
            ResetSlots();
        }
    }

    private bool AreNumbersEqual(int[] a, int[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    private void ResetSlots()
    {
        foreach (var slot in letterSlots) slot.text = "";
        foreach (var slot in numberSlots) slot.text = "";
        currentLetterIndex = 0;
        currentNumberIndex = 0;
        if (feedbackText) feedbackText.text = "";
    }

    private void ShuffleLetters()
    {
        // Fisher-Yates shuffle
        for (int i = 0; i < letterButtons.Length; i++)
        {
            int rand = Random.Range(i, letterButtons.Length);
            Button temp = letterButtons[i];
            letterButtons[i] = letterButtons[rand];
            letterButtons[rand] = temp;
        }

        // Clear old listeners và gán lại theo vị trí mới
        for (int i = 0; i < letterButtons.Length; i++)
        {
            letterButtons[i].onClick.RemoveAllListeners();
            int index = i;
            letterButtons[i].onClick.AddListener(() => AddLetter(targetLetters[index]));
        }
    }
}