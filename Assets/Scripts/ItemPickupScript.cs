using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ItemPickupScript : MonoBehaviour
{
    public enum pickupObject{ATTACK,HEAL,ESSENCE}
    public pickupObject currentObject;
    
    [Header("Item Identification")]
    [Tooltip("Unique ID for tracking item collection across scenes")]
    [SerializeField] private string itemId;
    
    [Header("Power-up Settings")]
    public int attackBoostAmount = 10;
    public float attackBoostDuration = 20f;

    [Header("Healing Setting")]
    public int healAmount = 25;
    public int essenceCount = 1;


    void Start()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            // Create a unique ID based on scene name, object name and position
            itemId = $"{gameObject.scene.name}_{gameObject.name}_{transform.position.x}_{transform.position.y}";
        }
        
        // Check if this item has already been collected
        GameManager gameManager = GameManager.instance;
        if (gameManager != null && gameManager.IsItemCollected(itemId))
        {
            // Item was already collected in a previous session, destroy it
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager gameManager = GameManager.instance;
            if (gameManager != null)
            {
                gameManager.MarkItemAsCollected(itemId);
            }

            PlayerManager playerManager = collision.GetComponent<PlayerManager>();
            if (playerManager == null)
            {
                Debug.LogWarning("Player doesn't have a PlayerManager component");
                Destroy(gameObject);
                return;
            }

            if (currentObject == pickupObject.ATTACK)
            {   
                // Find AttackScript in the SwordHitBox Gameobject (child of the player)
                AttackScript[] attackScripts = collision.GetComponentsInChildren<AttackScript>();
                if (attackScripts.Length > 0)
                {
                    SoundEffectManager.Play("Attack_Boost");
                    // Start coroutine for buff duration
                    if (playerManager != null)
                    {
                        playerManager.StartAttackBoost(attackScripts, attackBoostAmount, attackBoostDuration);
                    }
                    Debug.Log("Pickup attack orb - Attack increased by " + attackBoostAmount + " for " + attackBoostDuration + " seconds");
                }
                else
                {
                    Debug.LogWarning("Player doesn't have any AttackScript components in children");
                }
            }
            else if (currentObject == pickupObject.HEAL)
            {
               // Get the player's damageable script
                ScriptDamageable damageableScript = collision.GetComponent<ScriptDamageable>();
                if (damageableScript != null)
                {
                    SoundEffectManager.Play("Heal");
                    damageableScript.Heal(healAmount);
                    Debug.Log("Pickup healing orb - Player healed");
                }
                else
                {
                    Debug.LogWarning("Player doesn't have a ScriptDamageable component");
                }
            }
            else if (currentObject == pickupObject.ESSENCE)
            {
                playerManager.CollectEssences(essenceCount);
                // SoundEffectManager.Play("Essence");
                Debug.Log($"Essence collected. Total: {playerManager.essences}");
            }
            Destroy(gameObject);
        }
    }
}
