using UnityEngine;

[CreateAssetMenu(fileName = "HealthPotion", menuName = "Items/Health Potion")]
public class HealthPotion : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int healAmount = 20;

    public GameObject droppedPrefab;
}
