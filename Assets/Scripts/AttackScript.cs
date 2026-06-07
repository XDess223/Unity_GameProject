using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockbackDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        ScriptDamageable damageableScript = collision.GetComponent<ScriptDamageable>();

        if (damageableScript != null)
        {

            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? 
            knockbackDirection : new Vector2 (-knockbackDirection.x, knockbackDirection.y);
            bool gotHit = damageableScript.Hit(attackDamage, deliveredKnockback);

            if (gotHit)
            {
                Debug.Log(collision.name + " hit for " + attackDamage);
            }

        }

        DestroyableItemScript destroyableScript = collision.GetComponent<DestroyableItemScript>();
        if (destroyableScript != null)
        {
            destroyableScript.TakeDamage(attackDamage, this.gameObject);
            Debug.Log(collision.name + " destroyable hit for " + attackDamage);
        }
    }
}
