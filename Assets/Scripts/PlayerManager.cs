using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;


[RequireComponent(typeof(Rigidbody2D), typeof(ScriptTouchDirection), typeof(ScriptDamageable))]
public class PlayerManager : MonoBehaviour
{
    [Header("Game Component")]
    public GameObject playerObj;
    private Vector2 startScale;
    private Rigidbody2D player;
    ScriptTouchDirection touchDirectionScript;
    ScriptDamageable damageableScript;
    public GameManager gameManager;
    [Header("Essences Pickup")]
    public int essences;

    [Header("Power-up Effects")]
    public bool attackBoostActive = false;
    public float attackBoostTimeRemaining = 0f;

    [Header("Buff UI")]
    [SerializeField] private GameObject buffDisplayPanel;
    [SerializeField] private TMP_Text buffDurationText;
    
    [Header("Player Movement Component")]
    private float direction = 0f;
    private Animator anim;
    [SerializeField] private bool _isMoving = false;
    public bool IsMoving {
        get { return _isMoving; }
        private set 
        { 
            _isMoving = value;
            anim.SetBool(AnimationStrings.isMoving, _isMoving);
        }
    }
    [SerializeField] private bool _isFacingRight = true;
    public bool IsFacingRight {
        get { return _isFacingRight; }
        private set
        {
            _isFacingRight = value;
        }
    }
    [SerializeField] private bool _isRunning = false;
    public bool IsRunning {
        get { return _isRunning; }
        private set
        {
            _isRunning = value;
            anim.SetBool(AnimationStrings.isRunning, _isRunning);
        }
    }
    public bool CanMove
    {
        get
        {
            return anim.GetBool(AnimationStrings.canMove);
        }
    }
    public bool IsAlive
    {
        get
        {
            return anim.GetBool(AnimationStrings.isAlive);
        }
    }

    [Header("Player Component")]
    public float jumpHeight = 10f;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 5f;
    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {                
                if (IsMoving && !touchDirectionScript.IsOnWall) //
                {
                    if (touchDirectionScript.IsGround)
                    {
                        if (IsRunning)
                            return runSpeed;
                        else
                            return walkSpeed;
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            
            }
            else
            {
                return 0;
            }
        }
    }

    public float AttackCooldown
    {
        get
        {
            return anim.GetFloat(AnimationStrings.attackCooldown);
        }
        set
        {
            anim.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    [Header("Respawn Component")]
    public Vector3 respawnPoint;
    public GameObject deathZone;

    void Awake()
    {
        gameManager = GameManager.instance;
        if (buffDisplayPanel != null)
            buffDisplayPanel.SetActive(false);
    }

    void Start()
    {
        player = GetComponent<Rigidbody2D> ();
        anim = GetComponent<Animator> ();
        touchDirectionScript = GetComponent<ScriptTouchDirection> ();
        damageableScript = GetComponent<ScriptDamageable> ();


        startScale = playerObj.transform.localScale;
        
        if (respawnPoint == Vector3.zero)
        {
            respawnPoint = transform.position;
        }

    }

    void Update()
    {
        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

        bool isInAttackState = anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack");

        if (Input.GetButtonDown("Attack") && !isInAttackState && AttackCooldown <= 0 && touchDirectionScript.IsGround)
        {
            anim.SetTrigger(AnimationStrings.attackTrigger);
        }

        if (Input.GetButtonDown("Jump") && touchDirectionScript.IsGround && !isInAttackState)
        {
            anim.SetTrigger(AnimationStrings.jumpTrigger);

            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpHeight);
        }


        if (Input.GetButtonDown("Run"))
        {
            IsRunning = true;
        }
        else if (Input.GetButtonUp("Run"))
        {
            IsRunning = false;
        }
        
        // Check if player is moving
        direction = Input.GetAxis("Horizontal");
        IsMoving = direction != 0f;
             
        if (gameManager.confirmExit == true)
        {
            player.linearVelocity = Vector2.zero;

            anim.SetBool(AnimationStrings.isMoving, false);
            
            this.enabled = false;
        }

    }

    void FixedUpdate()
    {
        // player.linearVelocity = new Vector2(moveInput.x * speed, player.linearVelocity.y); //For TUtorial
        if (!damageableScript.LockVelocity)
        {    
            direction = Input.GetAxis("Horizontal"); 

            if (direction > 0f)
            { // Go Right
                player.linearVelocity = new Vector2(direction * CurrentMoveSpeed, player.linearVelocity.y);
                //Turn Right
                transform.localScale = new Vector2(startScale.x, startScale.y);
            }
            else if (direction < 0f)
            { // Go Left
                player.linearVelocity = new Vector2(direction * CurrentMoveSpeed, player.linearVelocity.y);
                // Turn Left
                transform.localScale = new Vector2(-startScale.x, startScale.y);
            }
            else
            { // Stay still
                player.linearVelocity = new Vector2(0, player.linearVelocity.y);
            }
        }

        anim.SetFloat(AnimationStrings.yVelocity, player.linearVelocity.y);
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        player.linearVelocity = new Vector2(knockback.x, player.linearVelocity.y + knockback.y);
    }

    public void StartAttackBoost(AttackScript[] attackScripts, int boostAmount, float duration)
    {
        // Start the coroutine to temporarily boost attack
        StartCoroutine(TemporaryAttackBoost(attackScripts, boostAmount, duration));
    }

    private IEnumerator TemporaryAttackBoost(AttackScript[] attackScripts, int boostAmount, float duration)
    {
        // Store original value
        Dictionary<AttackScript, int> originalValues = new Dictionary<AttackScript, int>();
        
        // Apply boost loop (If I want to add range)
        foreach (AttackScript attackScript in attackScripts)
        {
            originalValues[attackScript] = attackScript.attackDamage;
            attackScript.attackDamage += boostAmount;
        }
        
        // Set boost status
        attackBoostActive = true;
        attackBoostTimeRemaining = duration;

        // Show buff UI
        if (buffDisplayPanel != null)
        {
            buffDisplayPanel.SetActive(true);
        }
        
        // Wait
        while (attackBoostTimeRemaining > 0)
        {
            // Update the UI text with remaining time
            if (buffDurationText != null)
            {
                int minutes = Mathf.FloorToInt(attackBoostTimeRemaining / 60);
                int seconds = Mathf.FloorToInt(attackBoostTimeRemaining % 60);
                buffDurationText.text = string.Format("Atk Boost: {0:00}:{1:00}", minutes, seconds);
            }
            attackBoostTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        // Restore original values
        foreach (AttackScript attackScript in attackScripts)
        {
            attackScript.attackDamage = originalValues[attackScript];
        }

        // Reset boost status
        attackBoostActive = false;
        attackBoostTimeRemaining = 0f;

        buffDisplayPanel.SetActive(false);
        Debug.Log("Attack boost ended");
    }

    public void CollectEssences(int amount)
    {
        essences += amount;
        gameManager.playerEssences = essences;

        Debug.Log($"Collected {amount} essences. Total: {essences}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Touch DeathZone");
            int usedLives = gameManager.UpdateUsedLives();

            if (usedLives < gameManager.MaxLives)
            {
                transform.position = respawnPoint;
                player.linearVelocity = Vector2.zero;

                if (damageableScript != null)
                {
                    damageableScript.SetHealth(gameManager.playerMaxHealth, gameManager.playerMaxHealth);
                    gameManager.playerCurrentHealth = gameManager.playerMaxHealth;
                    Debug.Log("Health reset to full after death");
                }

            }
            else
            {
                gameObject.SetActive(false);
            }


        }
        else if (collision.gameObject.CompareTag("Door_Next"))
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            Vector3 leftOfDoorPosition = transform.position - new Vector3(2f, 0, 0);
            
            gameManager.SaveDoorPosition(currentScene, leftOfDoorPosition);
            gameManager.previousSceneIndex = currentScene;

            Debug.Log($"Going to next level, saved return position at {leftOfDoorPosition} for scene {currentScene}");

            gameManager.CheckWinGame();

            if (gameManager.isGameOver == true)
            {
                player.linearVelocity = Vector2.zero;
                anim.SetBool(AnimationStrings.isMoving, false);
                this.enabled = false;
            }
            else if (gameManager.finishedGame == true)
            {
                player.linearVelocity = Vector2.zero;
                anim.SetBool(AnimationStrings.isMoving, false);
                this.enabled = false;
            }
        }
        else if (collision.gameObject.CompareTag("Door_Back"))
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            int previousScene = currentScene - 1;
                        
            Debug.Log($"Going back to previous level {previousScene}");
            gameManager.previousSceneIndex = currentScene;
            gameManager.PreviousLevel();
        }

    }
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
        gameManager = GameManager.instance;
        
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
            Debug.Log("GameManager found on scene load");
        }
        
        if (respawnPoint == Vector3.zero)
        {
            respawnPoint = transform.position;
        }

        // Find UI references if needed
        if (buffDisplayPanel == null || buffDurationText == null)
        {
            GameObject panelObj = GameObject.FindWithTag("BuffDisplayPanel");
            if (panelObj != null)
            {
                buffDisplayPanel = panelObj;
                
                // Find the text component inside the panel
                TMP_Text[] textComponents = panelObj.GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text text in textComponents)
                {
                    if (text.gameObject.CompareTag("BuffDurationText"))
                    {
                        buffDurationText = text;
                        break;
                    }
                }
            }
            
            if (buffDisplayPanel != null)
            {
                buffDisplayPanel.SetActive(attackBoostActive);
            }
            
            Debug.Log("Buff UI references restored on scene load");
        }
        
        // Update health from GameManager if needed
        if (gameManager != null && damageableScript != null)
        {
            damageableScript.SetHealth(gameManager.playerCurrentHealth, gameManager.playerMaxHealth);

            essences = gameManager.playerEssences;
        }
    
    }
}
