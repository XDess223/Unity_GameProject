using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptCliffDetectionZone : MonoBehaviour
{
    public UnityEvent noCollisionRemain;
    public List<Collider2D> detectedCollision;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedCollision.Add(collision); 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedCollision.Remove(collision);
        
        if (detectedCollision.Count <= 0)
        {
            noCollisionRemain.Invoke();
        }
    }
}
