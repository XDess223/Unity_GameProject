using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(ScriptTouchDirection), typeof(Animator))]
[RequireComponent(typeof(ScriptDamageable), typeof(EnemyDropManager))]
public class EnemyLineOfSightScript : MonoBehaviour
{
    public PlayerManager playerManager;
    [Header("Enemy Line Of Sight Component")]
    [SerializeField]
    Transform castPoint;
    [SerializeField] private Transform player;
    [SerializeField] private float aggroRange;
    public LayerMask canSeeLayer;
    public float moveSpeed;
    private bool isAggro = false;
    [SerializeField] bool IsFacingLeft;
    [SerializeField] private bool isSearching;
    [SerializeField]private float attackRange = 1.5f; 
    Rigidbody2D rb2d;
    Animator animator;
    private ScriptTouchDirection touchingScript;
    private ScriptDamageable damageableScript;
    float startScale => Mathf.Abs(transform.localScale.x);
    [Header("Bool Component for Animator")]
    private bool _isCliffDetected = false;
    public bool IsCliffDetected
    {
        get { return _isCliffDetected; }
        private set
        {
            _isCliffDetected = value;
            animator.SetBool("isCliffDetected", _isCliffDetected);
        }
    }
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }
    [SerializeField] private bool _isMoving = false;
    public bool IsMoving {
        get { return _isMoving; }
        private set 
        { 
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, _isMoving);
        }
    }
    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, _hasTarget);
        }
    }
    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingScript = GetComponent<ScriptTouchDirection>();
        damageableScript = GetComponent<ScriptDamageable>();

        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        // distance to player
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
        // // Debug.Log(distToPlayer);
        if (!damageableScript.LockVelocity)
        {
            if (CanMove)
            {
                if (touchingScript.IsGround && !IsCliffDetected)
                {
                    if (CanSeePlayer(aggroRange))
                    {
                        isAggro = true;
                    }
                    else
                    {
                        if (isAggro)
                        {
                            if (!isSearching)
                            {
                                isSearching = true;
                                Invoke("StopChase", 3.5f);
                            }
                        }
                    }

                    if (isAggro)
                    {
                        if (distToPlayer <= attackRange)
                        {
                            TryAttack();
                        }
                        else
                        {
                            ChasePlayer();
                        }
                    }
                }
                else
                {
                    StopChase();
                }
            }
        }

    }

    private void TryAttack()
    {
        float currentCooldown = animator.GetFloat("attackCooldown");
        
        if (currentCooldown <= 0)
        {
            FacePlayer();
            
            animator.SetTrigger("attack");
        }
    }

    private void FacePlayer()
    {
        // Make sure enemy faces the player when attacking
        if (transform.position.x < player.position.x)
        {// Face right  
            transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);
        }
        else if (transform.position.x > player.position.x)
        { // Face left
            transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = transform.localScale;
        }
    }

    private void ChasePlayer()
    {
        float xPositionDif = player.position.x - transform.position.x;
        float threshold = 0.1f;
        
        if (Mathf.Abs(xPositionDif) < threshold)
        {
            rb2d.linearVelocity = new Vector2(0, rb2d.linearVelocity.y);
            IsMoving = false;
        }
        else if (xPositionDif > 0)
        {// Enemy on left side of player
            rb2d.linearVelocity = new Vector2(moveSpeed, 0);
            transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);
            IsFacingLeft = false;
            IsMoving = true;
        }
        else
        {// Enemy on right side of player
            rb2d.linearVelocity = new Vector2(-moveSpeed, 0);
            transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
            IsFacingLeft = true;
            IsMoving = true;
        }
    }
    private void StopChase()
    {
        isAggro = false;
        isSearching = false;
        IsMoving = false;
        rb2d.linearVelocity = new Vector2(0, rb2d.linearVelocity.y);
    }
    
    bool CanSeePlayer(float distance)
    {
        bool val = false;
        float castDistance = distance;
        if (IsFacingLeft)
        {
            castDistance = -distance;
        }
        Vector2 endPos = castPoint.position + Vector3.right * castDistance;

        RaycastHit2D hit2D = Physics2D.Linecast(transform.position, endPos, canSeeLayer);

        if (hit2D.collider != null)
        {
            if (hit2D.collider.gameObject.CompareTag("Player"))
            {// Agro
                val = true;
                Debug.DrawLine(castPoint.position, hit2D.point, Color.red);
            }
            else
            {
                val = false;
                Debug.DrawLine(castPoint.position, hit2D.point, Color.yellow);
            }
            

        }
        else
        {
            Debug.DrawLine(castPoint.position, endPos, Color.blue);
        }
        return val;
    }

    void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

        // Helper method to find the player
    void FindPlayer()
    {
        playerManager = FindAnyObjectByType<PlayerManager>();
        
        if (playerManager != null)
        {
            player = playerManager.transform;
            Debug.Log("Player found: " + player.name);
        }
        else
        {
            Debug.LogWarning("PlayerManager not found in the scene!");
        }
    }

    // For debugging - visualize the detection ranges
    void OnDrawGizmosSelected()
    {
        // Draw agro range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void OnCliffDetected()
    {
        Debug.Log("Cliff detected!");
        IsCliffDetected = true;
        HasTarget = false;
        StopChase();
        
        StartCoroutine(ResetCliffDetection());
    }

    private IEnumerator ResetCliffDetection()
    {
        yield return new WaitForSeconds(2.0f);
        IsCliffDetected = false;

        rb2d.linearVelocity = new Vector2(-moveSpeed, 0);
        transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        rb2d.linearVelocity = new Vector2(knockback.x, rb2d.linearVelocity.y + knockback.y);
        // Get player direction
        bool playerIsOnRight = player.position.x > transform.position.x;
        
        // Check if enemy is facing away from the player
        if ((playerIsOnRight && IsFacingLeft) || (!playerIsOnRight && !IsFacingLeft))
        {
            // Enemy is facing away from player, flip to face player
            IsFacingLeft = !IsFacingLeft;
            transform.localScale = new Vector3(IsFacingLeft ? -startScale : startScale, transform.localScale.y, transform.localScale.z);
        }

        // Make the enemy aware of player and start chasing
        isAggro = true;
        isSearching = false;
        // CancelInvoke("StopChase");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
