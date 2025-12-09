using UnityEngine;

public class LootDrop : MonoBehaviour
{
    [Header("Loot Settings")]
    public GameObject[] lootItems; 
    public float dropChance = 1f; 

    void BreakObject()
    {
        DropLoot();
    }

    private void DropLoot()
    {
        if (lootItems == null || lootItems.Length == 0)
            return;

        if (Random.value > dropChance)
            return;

        int index = Random.Range(0, lootItems.Length);

        Instantiate(lootItems[index], transform.position, Quaternion.identity);
    }
}
