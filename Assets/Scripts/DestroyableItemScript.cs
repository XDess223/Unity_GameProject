using UnityEngine;
using System.Collections.Generic;

public class DestroyableItemScript : MonoBehaviour
{
    [Header("Destruction Settings")]
    [SerializeField] private int health = 3;
    [SerializeField] private bool destroyOnImpact = false;
    [SerializeField] private bool requiresPlayerAttack = true;
    
    [Header("Drop Settings")]
    [SerializeField] private bool shouldDropItems = true;
    [SerializeField] private GameObject healOrbPrefab;
    [SerializeField] private GameObject attackOrbPrefab;
    [SerializeField] private DropMode dropMode = DropMode.Random;
    [SerializeField] private ItemPickupScript.pickupObject specificDropType;
    public enum DropMode {Random, Specific};
    [SerializeField] private List<DropItem> possibleDrops = new List<DropItem>();
    [Header("VFX")]
    [SerializeField] private GameObject destructionVFX;
    [SerializeField] private LayerMask playerHitBoxLayer;
    
    [System.Serializable]
    public class DropItem
    {
        public ItemPickupScript.pickupObject type;
        [Range(0f, 1f)]
        public float dropChance = 0.5f;
    }

    private void Start()
    {
        
        // Add default drops if list is empty
        if (possibleDrops.Count == 0)
        {
            DropItem healDrop = new DropItem();
            healDrop.type = ItemPickupScript.pickupObject.HEAL;
            healDrop.dropChance = 0.5f;
            
            DropItem attackDrop = new DropItem();
            attackDrop.type = ItemPickupScript.pickupObject.ATTACK;
            attackDrop.dropChance = 0.5f;
                
            possibleDrops.Add(healDrop);
            possibleDrops.Add(attackDrop);
        }
    }
    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    public void TakeDamage(int damageAmount, GameObject damageSource)
    {
        // Check if damage source is player and require player attacks
        if (requiresPlayerAttack && !IsInLayerMask(damageSource.layer, playerHitBoxLayer))
        {
            return;
        }
        
        health -= damageAmount;
        
        SoundEffectManager.Play("Crate");
        
        if (health <= 0)
        {
            Destroy();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (destroyOnImpact)
        {
            GameObject collider = collision.gameObject;
            if (requiresPlayerAttack && IsInLayerMask(collider.layer, playerHitBoxLayer))
            {
                Destroy();
            }
            else if (!requiresPlayerAttack)
            {
                Destroy();
            }
        }
    }
    
    public void Destroy()
    {
        SoundEffectManager.Play("Crate");
        
        // Spawn destruction VFX
        if (destructionVFX != null)
        {
            Instantiate(destructionVFX, transform.position, Quaternion.identity);
        }
        
        // Drop items
        if (shouldDropItems)
        {
            DropItems();
        }
        
        // Destroy the object
        Destroy(gameObject);
    }
    
    private void DropItems()
    {
        ItemPickupScript.pickupObject selectedType;
        
        if (dropMode == DropMode.Specific)
        {
            // Use the specifically chosen drop type
            selectedType = specificDropType;
        }
        else // Random mode
        {
            // Create a weighted selection list
            List<ItemPickupScript.pickupObject> weightedList = new List<ItemPickupScript.pickupObject>();
            
            foreach (DropItem item in possibleDrops)
            {
                int weight = Mathf.RoundToInt(item.dropChance * 100);
                for (int i = 0; i < weight; i++)
                {
                    weightedList.Add(item.type);
                }
            }
            
            if (weightedList.Count == 0) return;
            
            selectedType = weightedList[Random.Range(0, weightedList.Count)];
        }
        
        GameObject lootPrefab = null;
        switch (selectedType)
        {
            case ItemPickupScript.pickupObject.HEAL:
                lootPrefab = healOrbPrefab;
                break;
            case ItemPickupScript.pickupObject.ATTACK:
                lootPrefab = attackOrbPrefab;
                break;
        }
        
        if (lootPrefab != null)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.3f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            GameObject loot = Instantiate(lootPrefab, spawnPosition, Quaternion.identity);
            
            Rigidbody2D lootRb = loot.GetComponent<Rigidbody2D>();
            if (lootRb != null)
            {
                float popForce = Random.Range(3f, 6f);
                Vector2 popDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
                lootRb.AddForce(popDirection * popForce, ForceMode2D.Impulse);
            }
        }
    }
}
