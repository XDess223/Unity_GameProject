using UnityEngine;

public class MoveableItemScript : MonoBehaviour
{
    [Header("Crate Properties")]
    public bool canBePushed = true;
    public bool canBePulled = true;
    
    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jointBreakForce = 1000f;
    [SerializeField] private float jointDistance = 0.5f;
    
    [HideInInspector] public bool isBeingMoved = false;
    
    private PlayerMoveItemHandler playerHandler;
    private FixedJoint2D fixedJoint;
    private Vector2 jointAnchor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
            rb.mass = 2f;
            rb.linearDamping = 3f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    
    public void SetPlayerHandler(PlayerMoveItemHandler handler)
    {
        playerHandler = handler;
    }
    
    public void AttachToPlayer(Rigidbody2D playerRb, bool isPushing)
    {
        if (isBeingMoved || fixedJoint != null)
            return;
                
        // Create fixed joint if it doesn't exist
        fixedJoint = gameObject.AddComponent<FixedJoint2D>();
        fixedJoint.connectedBody = playerRb;
        fixedJoint.breakForce = jointBreakForce;
        fixedJoint.enabled = true;  // Make sure it's enabled
        
        // Set the joint anchor point based on pushing or pulling
        if (isPushing)
        {
            // When pushing, joint is on the side facing the player
            jointAnchor = (playerRb.transform.position.x > transform.position.x) ? 
                        new Vector2(-jointDistance, 0.5f) : new Vector2(jointDistance, 0.5f);
        }
        else
        {
            // When pulling, joint is on the side away from the player
            jointAnchor = (playerRb.transform.position.x > transform.position.x) ? 
                        new Vector2(jointDistance, 0.5f) : new Vector2(-jointDistance, 0.5f);
        }
        
        // Adjust Y position to match player's center
        jointAnchor.y = 0.5f;  // Raise the joint point up from the ground
        
        fixedJoint.anchor = jointAnchor;
        isBeingMoved = true;
    }
    
    public void StopMoving()
    {
        isBeingMoved = false;
        playerHandler = null;
        
        // Remove the joint when stopping
        if (fixedJoint != null)
        {
            Destroy(fixedJoint);
            fixedJoint = null;
        }
        
        // Ensure the crate stops moving
        rb.linearVelocity = Vector2.zero;
    }
}
