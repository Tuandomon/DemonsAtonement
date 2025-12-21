using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LockZone : MonoBehaviour
{
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();

        // M?C ??NH: M?
        col.isTrigger = true;

        // N?u b?m X (ch?a win) ? B? CH?N
        if (PlayerPrefs.GetInt("LockZone1", 0) == 1)
        {
            col.isTrigger = false;
        }
    }
}
