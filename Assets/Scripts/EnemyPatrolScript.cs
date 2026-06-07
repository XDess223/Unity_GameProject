using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(ScriptTouchDirection), typeof(Animator))]
[RequireComponent(typeof(ScriptDamageable), typeof(EnemyDropManager))]
public class EnemyPatrolScript : MonoBehaviour
{
    [Header("Enemy Component")]
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;

    private Rigidbody2D enemy;
    private ScriptTouchDirection touchingScript;
    public ScriptPlayerDetectionZone playerDetectionZoneScript;
    private ScriptDamageable damageableScript;
    private Animator anim;
    public enum WalkableDirecton {Right, Left}
    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirecton _walkDirection;
    public WalkableDirecton WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, 
                gameObject.transform.localScale.y);
                // GetComponent<SpriteRenderer>().flipX != GetComponent<SpriteRenderer>().flipX;

                if (value == WalkableDirecton.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirecton.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }
    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            anim.SetBool(AnimationStrings.hasTarget, _hasTarget);
        }
    }
    public bool CanMove
    {
        get
        {
            return anim.GetBool(AnimationStrings.canMove);
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        touchingScript = GetComponent<ScriptTouchDirection>();
        damageableScript = GetComponent<ScriptDamageable>(); 
    }

    void Update()
    {
        HasTarget = playerDetectionZoneScript.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (touchingScript.IsGround && touchingScript.IsOnWall)
        {
            FlipDirection();
        }
        
        if (!damageableScript.LockVelocity)
        {
            if (CanMove && !touchingScript.IsOnWall)
            {
                enemy.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, enemy.linearVelocity.y);
            }
            else
            {
                enemy.linearVelocity = new Vector2(Mathf.Lerp(enemy.linearVelocity.x, 0, walkStopRate), enemy.linearVelocity.y);
            }
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirecton.Right)
        {
            WalkDirection = WalkableDirecton.Left;
        }
        else if (WalkDirection == WalkableDirecton.Left)
        {
            WalkDirection = WalkableDirecton.Right;
        }
        else
        {
            Debug.Log("Current walkable direction is not set to legal values of left or right");
        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        enemy.linearVelocity = new Vector2(knockback.x, enemy.linearVelocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if (touchingScript.IsGround)
        {
            FlipDirection();
        }
    }
}
