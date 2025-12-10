using UnityEngine;

public class SuggestPageController : MonoBehaviour
{
    [Header("Pages")]
    public GameObject page1;   // Image_hien chu goi y 1
    public GameObject page2;   // Image_hien chu goi y 2

    private int currentPage = 1;

    void Start()
    {
        ShowPage(1); // mặc định mở trang 1
    }

    public void NextPage()
    {
        if (currentPage == 1)
        {
            ShowPage(2);
        }
    }

    public void PrevPage()
    {
        if (currentPage == 2)
        {
            ShowPage(1);
        }
    }

    // ✨ Hàm reset về trang đầu khi nhấn nút Thoát
    public void ResetToFirstPage()
    {
        ShowPage(1);
    }

    // Đã chuyển thành public để script khác gọi được
    public void ShowPage(int page)
    {
        currentPage = page;

        if (page1 != null) page1.SetActive(page == 1);
        if (page2 != null) page2.SetActive(page == 2);
    }
}
