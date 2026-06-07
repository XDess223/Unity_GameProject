using UnityEngine;

public class EnemyDropManager : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private GameObject essencePrefab;
    [Range(0f, 1f)]
    [SerializeField] private float essenceDropChance = 1f;
    [SerializeField] private int minEssenceDrops = 1;
    [SerializeField] private int maxEssenceDrops = 3;
    [SerializeField] private float dropSpreadRadius = 0.5f;
    
    private ScriptDamageable damageableScript;
    
    private void Start()
    {
        damageableScript = GetComponent<ScriptDamageable>();
        
        // Subscribe to health changes by observing IsAlive property
        if (damageableScript != null)
        {
            // Using InvokeRepeating as a workaround to check enemy alive status
            InvokeRepeating("CheckEnemyStatus", 0.1f, 0.1f);
        }
        else
        {
            Debug.LogError("ScriptDamageable component is missing!");
        }
    }
    
    private bool wasAlive = true;
    
    private void CheckEnemyStatus()
    {
        if (wasAlive && !damageableScript.IsAlive)
        {
            // Enemy just died
            DropItems();
        }
        
        wasAlive = damageableScript.IsAlive;
    }
    
    private void DropItems()
    {
        // Roll for essence drops
        if (Random.value <= essenceDropChance)
        {
            int essenceCount = Random.Range(minEssenceDrops, maxEssenceDrops + 1);
            for (int i = 0; i < essenceCount; i++)
            {
                SpawnDrop(essencePrefab, ItemPickupScript.pickupObject.ESSENCE);
            }
        }
    }
    
    private void SpawnDrop(GameObject prefab, ItemPickupScript.pickupObject pickupType)
    {
        if (prefab == null) 
        {
            Debug.LogWarning("Drop prefab is missing!");
            return;
        }
        
        // Calculate random position within spread radius
        Vector2 randomOffset = Random.insideUnitCircle * dropSpreadRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        
        // Instantiate the drop
        GameObject drop = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        // Set the pickup type if it has an ItemPickupScript
        ItemPickupScript pickupScript = drop.GetComponent<ItemPickupScript>();
        if (pickupScript != null)
        {
            pickupScript.currentObject = pickupType;
        }
    }
}
