using UnityEngine;

public class TransformSkill : MonoBehaviour
{
    public GameObject currentCharacter;     // Nhân vật hiện tại
    public GameObject transformedCharacter; // Nhân vật sau khi biến hình
    private bool isTransformed = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ActivateTransformSkill();
        }
    }

    void ActivateTransformSkill()
    {
        if (!isTransformed)
        {
            // Ẩn nhân vật hiện tại
            currentCharacter.SetActive(false);

            // Hiện nhân vật biến hình
            transformedCharacter.SetActive(true);

            isTransformed = true;
            Debug.Log("Đã biến hình!");
        }
        else
        {
            // Quay lại nhân vật ban đầu
            transformedCharacter.SetActive(false);
            currentCharacter.SetActive(true);

            isTransformed = false;
            Debug.Log("Đã trở lại hình dạng ban đầu!");
        }
    }
}