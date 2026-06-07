using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ScriptDamageable : MonoBehaviour
{
    public UnityEvent<float, Vector2> damageableHit;
    public UnityEvent<float> damageableHealed;
    private Animator anim;
    public GameObject healthBar;
    public Slider healthBarSlider;
    public TMP_Text healthText;

    GameManager gameManager => GameManager.instance;


    [SerializeField] private float _maxHealth = 100f;
    public float MaxHealth
    {
        get { return _maxHealth; }
        private set
        {
            _maxHealth = value;
        }
    }
    [SerializeField] private float _health = 100f;
    public float Health
    {
        get { return _health; }
        private set
        {
            _health = value;
            if (_health <= 0f)
            {
                IsAlive = false;

                                // Handle player death by enemy attack
                if (gameObject.CompareTag("Player") && gameManager != null)
                {
                    int usedLives = gameManager.UpdateUsedLives();
                    
                    if (usedLives < gameManager.MaxLives)
                    {
                        // Small delay before respawn to allow death animation to play
                        StartCoroutine(RespawnAfterDelay(0.8f));
                    }
                }
            }
        }
    }
    
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            anim.SetBool(AnimationStrings.isAlive, _isAlive);
        }
    }
    
    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public bool LockVelocity
    {
        get
        {
            return anim.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            anim.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        
        if (gameObject.CompareTag("Player"))
        {

            if (gameManager != null)
            {
                if (gameManager.playerCurrentHealth != 100f)
                {
                    Health = gameManager.playerCurrentHealth;
                    MaxHealth = gameManager.playerMaxHealth;
                }
                else
                {
                    gameManager.playerCurrentHealth = Health;
                    gameManager.playerMaxHealth = MaxHealth;
                }
            }
            SetHealthUI();
        }
    }

    public bool Hit(float damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;

            if (Health <= 0)
            {
                SoundEffectManager.Play("Death");
            }
            else
            {
                if (gameObject.CompareTag("Player"))
                {
                    SoundEffectManager.Play("Player_Hit");
                }
                if (gameObject.CompareTag("Enemy"))
                {
                    SoundEffectManager.Play("Enemy_Hit");
                }
            }

            isInvincible = true;
            anim.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            healthBar.SetActive(true);
            healthBarSlider.value = CalculatePencentHealth();
            if (gameObject.CompareTag("Player"))
            {
                SetHealthUI();
            }
            
            return true;
        }

        return false;
    }

    public void Heal(float amount)
    {
        if (IsAlive)
        {
            // Calculate how much health will actually be restored (can't exceed max health)
            float healthBefore = _health;
            _health = Mathf.Min(_health + amount, _maxHealth);
            float actualHealing = _health - healthBefore;
            
            if (gameObject.CompareTag("Player") && gameManager != null)
            {
                gameManager.playerCurrentHealth = _health;
            }

            // Only trigger event if healing actually occurred
            if (actualHealing > 0)
            {
                // Invoke the healing event if there are subscribers
                damageableHealed?.Invoke(actualHealing);
                healthBarSlider.value = CalculatePencentHealth();
                if (gameObject.CompareTag("Player"))
                {
                    SetHealthUI();
                }
                
            }
        }
    }

    public void SetHealth(float newHealth, float newMaxHealth = -1)
    {
        Health = Mathf.Min(newHealth, MaxHealth);
        
        if (newMaxHealth > 0)
        {
            MaxHealth = newMaxHealth;
        }

        if (gameObject.CompareTag("Player") && gameManager != null)
        {
            gameManager.playerCurrentHealth = Health;
            
            if (newMaxHealth > 0)
            {
                gameManager.playerMaxHealth = MaxHealth;
            }
        }

        if (gameObject.CompareTag("Player"))
        {
            SetHealthUI();
        }
    }

    void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    float CalculatePencentHealth()
    {
        return Health / MaxHealth;
    }

    void SetHealthUI()
    {
        healthBarSlider.value = CalculatePencentHealth();
        healthText.text = "Health: " + Mathf.Ceil(Health).ToString() + "/" + Mathf.Ceil(MaxHealth).ToString();
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Find the player manager
        PlayerManager playerManager = GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            // Reset position to respawn point
            transform.position = playerManager.respawnPoint;
            
            // Reset velocity
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            // Reset health
            SetHealth(gameManager.playerMaxHealth, gameManager.playerMaxHealth);
            gameManager.playerCurrentHealth = gameManager.playerMaxHealth;
            
            // Reset alive state
            IsAlive = true;
            
            Debug.Log("Player respawned after death from enemy attack");
        }
    }


}

