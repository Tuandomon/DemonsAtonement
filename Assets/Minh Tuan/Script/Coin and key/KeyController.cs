using UnityEngine;
using UnityEngine.UI;

public class KeyController : MonoBehaviour
{
    private Image keyImage1;
    private Image keyImage2;
    private Image keyImage3;
    private Image keyImage4;

    private int keyCount = 0;

    private void Start()
    {
        // Tìm trực tiếp từng hình ảnh theo tên trong Canvas
        GameObject obj1 = GameObject.Find("ImageKey1");
        GameObject obj2 = GameObject.Find("ImageKey2");
        GameObject obj3 = GameObject.Find("ImageKey3");
        GameObject obj4 = GameObject.Find("ImageKey4");

        if (obj1 != null) keyImage1 = obj1.GetComponent<Image>();
        if (obj2 != null) keyImage2 = obj2.GetComponent<Image>();
        if (obj3 != null) keyImage3 = obj3.GetComponent<Image>();
        if (obj4 != null) keyImage4 = obj4.GetComponent<Image>();

        // Ẩn toàn bộ key khi bắt đầu
        if (keyImage1 != null) keyImage1.enabled = false;
        if (keyImage2 != null) keyImage2.enabled = false;
        if (keyImage3 != null) keyImage3.enabled = false;
        if (keyImage4 != null) keyImage4.enabled = false;

        // Hiển thị lại số key đã nhặt từ CoinandKeyManager
        keyCount = CoinandKeyManager.instance.totalKeys;
        if (keyCount >= 1 && keyImage1 != null) keyImage1.enabled = true;
        if (keyCount >= 2 && keyImage2 != null) keyImage2.enabled = true;
        if (keyCount >= 3 && keyImage3 != null) keyImage3.enabled = true;
        if (keyCount >= 4 && keyImage4 != null) keyImage4.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Key"))
        {
            keyCount++;

            if (keyCount == 1 && keyImage1 != null) keyImage1.enabled = true;
            else if (keyCount == 2 && keyImage2 != null) keyImage2.enabled = true;
            else if (keyCount == 3 && keyImage3 != null) keyImage3.enabled = true;
            else if (keyCount == 4 && keyImage4 != null) keyImage4.enabled = true;

            CoinandKeyManager.instance.AddKey(1);

            Destroy(collision.gameObject);
        }
    }
}