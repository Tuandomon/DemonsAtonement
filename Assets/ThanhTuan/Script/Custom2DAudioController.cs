using UnityEngine;

public class Custom2DAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    // Gán Main Camera (hoặc Game Object có Audio Listener) vào đây trong Inspector
    public Transform listenerTransform;

    // Thuộc tính để bạn điều chỉnh trong Inspector
    [Header("Speed/Loop Settings")]
    public float targetPitch = 1.0f;     // Tốc độ phát lại (1.0 = bình thường, 2.0 = gấp đôi)

    [Header("2D Range Settings")]
    public float maxRange = 5f;        // Phạm vi ảnh hưởng tối đa (tính bằng đơn vị Unity 2D)
    public float maxVolume = 1f;       // Âm lượng tối đa (thường là 1)

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Cần có thành phần Audio Source trên Game Object này!");
            return;
        }

        // Tắt tính toán 3D tự động của Unity để chúng ta điều khiển bằng code
        audioSource.spatialBlend = 0f;

        // Cài đặt tốc độ loop ban đầu
        audioSource.pitch = targetPitch;

        // Nếu bạn quên gán, cố gắng tìm Audio Listener trên Main Camera
        if (listenerTransform == null)
        {
            if (Camera.main != null)
            {
                listenerTransform = Camera.main.transform;
            }
        }

        audioSource.Play();
    }

    void Update()
    {
        if (listenerTransform == null || audioSource == null) return;

        // 1. Tính Khoảng cách 2D (Chỉ dùng X và Y để bỏ qua Z)
        Vector2 sourcePos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 listenerPos2D = new Vector2(listenerTransform.position.x, listenerTransform.position.y);

        float distance2D = Vector2.Distance(sourcePos2D, listenerPos2D);

        // 2. Tính toán Âm lượng dựa trên Khoảng cách
        if (distance2D >= maxRange)
        {
            // Ngoài phạm vi, âm lượng bằng 0
            audioSource.volume = 0f;
        }
        else
        {
            // Tính suy giảm tuyến tính (Linear Falloff):
            // 1 - (Phần trăm khoảng cách đã đi trong phạm vi)
            float distanceRatio = distance2D / maxRange;
            float calculatedVolume = 1f - distanceRatio;

            // Gán âm lượng (nhân với âm lượng tối đa ban đầu)
            audioSource.volume = calculatedVolume * maxVolume;
        }
    }
}