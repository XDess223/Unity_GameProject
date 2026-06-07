using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerManager))]
public class PlayerMoveItemHandler : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionRadius = 1.5f;
    public LayerMask crateLayer;
    public float moveSpeedReduction = 0.6f; // Player moves at 60% speed when moving crates
    
    [Header("Visual Feedback")]
    public GameObject interactionPrompt;

    [Header("References")]
    private PlayerManager playerManager;
    private Rigidbody2D rb;
    private MoveableItemScript currentCrate;
    private bool isMovingCrate = false;
    private float originalWalkSpeed;
    private float originalRunSpeed;
    private bool isPushing = false;
    
    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        // Store original speeds
        originalWalkSpeed = playerManager.walkSpeed;
        originalRunSpeed = playerManager.runSpeed;
    }
    
    private void Update()
    {
        if (!playerManager.IsAlive || !playerManager.CanMove)
        {
            if (isMovingCrate)
                ReleaseCrate();
            return;
        }
        
        // Check for crates nearby
        MoveableItemScript nearestCrate = FindNearestCrate();
        
        // Show prompt if near a crate and not already moving one
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(nearestCrate != null && !isMovingCrate);
        }
        
        // Handle interaction input
        if (Input.GetKeyDown(interactKey) && nearestCrate != null && !isMovingCrate)
        {
            GrabCrate(nearestCrate);
        }
        else if (Input.GetKeyUp(interactKey) && isMovingCrate)
        {
            ReleaseCrate();
        }
    }
    
    private MoveableItemScript FindNearestCrate()
    {
        // Return current crate if already moving one
        if (isMovingCrate && currentCrate != null)
            return currentCrate;
            
        // Use a higher position for the interaction check
        Vector2 interactionCenter = (Vector2)transform.position + new Vector2(0, 0.4f);
        
        // Find crates in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(interactionCenter, interactionRadius, crateLayer);
        
        if (colliders.Length == 0)
            return null;
            
        // Get the nearest one
        MoveableItemScript nearest = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider2D collider in colliders)
        {
            MoveableItemScript crate = collider.GetComponent<MoveableItemScript>();
            if (crate != null)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearest = crate;
                }
            }
        }
        
        return nearest;
    }
    
    private void GrabCrate(MoveableItemScript crate)
    {
        // Get facing direction (push or pull)
        isPushing = IsFacingCrate(crate.transform);
        
        // Check if crate allows this type of movement
        if (isPushing && !crate.canBePushed) return;
        if (!isPushing && !crate.canBePulled) return;
        
        currentCrate = crate;
        isMovingCrate = true;
        
        // Attach crate to player via fixed joint
        crate.AttachToPlayer(rb, isPushing);
        crate.SetPlayerHandler(this);
        
        // Slow down the player
        playerManager.walkSpeed *= moveSpeedReduction;
        playerManager.runSpeed *= moveSpeedReduction;
    }
    
    private void ReleaseCrate()
    {
        if (currentCrate != null)
        {
            currentCrate.StopMoving();
        }
        
        isMovingCrate = false;
        currentCrate = null;
        
        // Restore original speeds
        playerManager.walkSpeed = originalWalkSpeed;
        playerManager.runSpeed = originalRunSpeed;
    }
    
    private bool IsFacingCrate(Transform crateTransform)
    {
        Vector2 directionToCrate = (crateTransform.position - transform.position).normalized;
        float dotProduct = Vector2.Dot(transform.right * Mathf.Sign(transform.localScale.x), directionToCrate);
        
        return dotProduct > 0; // If positive, player is facing the crate
    }
    
    private void OnDrawGizmosSelected()
    {
            // Use the current transform for edit mode, or calculate based on scale for play mode
            Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
            Vector2 rayDirection = Vector2.right * Mathf.Sign(transform.localScale.x);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * interactionRadius);
    }
    
    private void OnDisable()
    {
        if (isMovingCrate)
            ReleaseCrate();
    }
}
