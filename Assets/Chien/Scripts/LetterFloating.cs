using UnityEngine;

public class LetterFloating : MonoBehaviour
{
    public float amplitude = 0.2f;   // độ cao dao động
    public float speed = 2f;         // tốc độ dao động

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
