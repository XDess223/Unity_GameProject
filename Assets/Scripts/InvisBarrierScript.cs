using UnityEngine;
using System.Collections;
using TMPro;

public class InvisBarrierScript : MonoBehaviour
{
    [Header("Barrier Settings")]
    [SerializeField] private int requiredEssence = 5;
    public ScriptPlayerDetectionZone playerDetectionZoneScript;
    [SerializeField] private TMP_Text requiredEssenceText;
    
    private BoxCollider2D barrierCollider;
    private bool isDisabled = false;
    private PlayerManager playerManager;
    public bool HasTarget;
    
    private void Start()
    {
        requiredEssenceText.enabled = false;
        barrierCollider = GetComponent<BoxCollider2D>();
        if (barrierCollider == null)
        {
            barrierCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        playerManager = FindAnyObjectByType<PlayerManager>();

    }
    
    private void Update()
    {
        HasTarget = playerDetectionZoneScript.detectedColliders.Count > 0;
        if (HasTarget && !isDisabled)
        {
            if (playerManager.essences < requiredEssence)
            {
                requiredEssenceText.text = $"Need {requiredEssence} essence to pass";
                requiredEssenceText.enabled = true;
            }
        }
        else
        {
            requiredEssenceText.enabled = false;
        }
        if (!isDisabled && playerManager != null)
        {

            if (playerManager.essences >= requiredEssence)
            {
                DisableBarrier();
                SoundEffectManager.Play("Barrier_Open");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        requiredEssenceText.enabled = false;
    }

    private void DisableBarrier()
    {
        isDisabled = true;
        barrierCollider.enabled = false;
        
    }
    
}
