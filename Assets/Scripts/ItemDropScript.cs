using UnityEngine;

public class ItemDropScript : MonoBehaviour
{
    private Rigidbody2D rd2d;
    private CircleCollider2D objCollider;
    public float dropForce = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objCollider = GetComponent<CircleCollider2D>();
        rd2d = GetComponent<Rigidbody2D>();
        rd2d.AddForce(Vector2.up * dropForce, ForceMode2D.Impulse);  
    }

}
