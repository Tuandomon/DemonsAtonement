using UnityEngine;
using System.Collections;

public class FireBreather : MonoBehaviour
{
    public GameObject fireHazardPrefab;
    public Transform spawnPoint;
    public float fireDuration = 2f;
    public float cooldownDuration = 3f;

    private GameObject currentFire;
    private readonly Quaternion Z_ROTATION_45 = Quaternion.Euler(0, 0, 45f);

    void Start()
    {
        StartCoroutine(FireCycle());
    }

    IEnumerator FireCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldownDuration);

            if (fireHazardPrefab != null && spawnPoint != null)
            {
                Quaternion finalRotation = spawnPoint.rotation * Z_ROTATION_45;

                currentFire = Instantiate(fireHazardPrefab, spawnPoint.position, finalRotation);
            }

            yield return new WaitForSeconds(fireDuration);

            if (currentFire != null)
            {
                Destroy(currentFire);
            }
        }
    }
}