using UnityEngine;
using System.Collections;

public class playerMoveObjectScript : MonoBehaviour
{
    public float distance = 1f;
    public LayerMask moveableItemLayer;
    GameObject moveItem;
    [Header("Static Component")]
    public bool beingMove;
    float xPos;

    void Start()
    {
        xPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (!beingMove)
        {
            transform.position = new Vector2(xPos, transform.position.y);
        }
        else
        {
            xPos = transform.position.x;
        }
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, moveableItemLayer);

        if (hit.collider != null && hit.collider.CompareTag("Moveable") && Input.GetKeyDown(KeyCode.E))
        {
            moveItem = hit.collider.gameObject;
            moveItem.GetComponent<FixedJoint2D>().enabled = true;
            beingMove = true;
            moveItem.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            moveItem.GetComponent<FixedJoint2D>().enabled = false;
            beingMove = false;
        }
        
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * transform.localScale.x * distance);
    }
}
