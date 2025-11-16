using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (teleData.useSpawnPoint && !string.IsNullOrEmpty(teleData.spawnPointName))
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            GameObject spawnObj = GameObject.Find(teleData.spawnPointName);

            if (player != null && spawnObj != null)
            {
                player.transform.position = spawnObj.transform.position;
            }

            // Reset flag
            teleData.useSpawnPoint = false;
            teleData.spawnPointName = "";
        }
    }
}
