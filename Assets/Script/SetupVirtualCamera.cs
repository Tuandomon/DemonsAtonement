using UnityEngine;
using Cinemachine;

public class SetupVirtualCamera : MonoBehaviour
{
    void Start()
    {
        // Tìm Virtual Camera trong scene
        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();

        if (vcam != null)
        {
            // Gán Player làm đối tượng cần theo dõi
            vcam.Follow = transform;
            vcam.LookAt = transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Cinemachine Virtual Camera trong scene!");
        }
    }
}