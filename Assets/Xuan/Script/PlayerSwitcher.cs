using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    public GameObject player;             // Đối tượng player
    public GameObject transformObject;    // Đối tượng thay thế (transform)
    public KeyCode switchKey = KeyCode.Z; // Phím để kích hoạt

    private bool isSwitched = false;
    private float switchTime = 15f;

    void Start()
    {
        // Đảm bảo transformObject ẩn khi bắt đầu
        transformObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey) && !isSwitched)
        {
            StartCoroutine(SwitchPlayer());
        }
    }

    System.Collections.IEnumerator SwitchPlayer()
    {
        isSwitched = true;

        // Lưu vị trí hiện tại của player
        Vector3 playerPosition = player.transform.position;

        // Ẩn player
        player.SetActive(false);

        // Di chuyển và hiện transformObject
        transformObject.transform.position = playerPosition;
        transformObject.SetActive(true);

        // Chờ 15 giây
        yield return new WaitForSeconds(switchTime);

        // Ẩn transformObject và hiện lại player
        transformObject.SetActive(false);
        player.SetActive(true);
        player.transform.position = playerPosition;

        isSwitched = false;
    }
}