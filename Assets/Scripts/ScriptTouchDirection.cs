using UnityEngine;

public class ScriptTouchDirection : MonoBehaviour
{
    public float groundDistance = 0.05f;
    public float wallDistance = 0.1f;
    public float cellingDistance = 0.05f;
    public ContactFilter2D castFilter;
    private BoxCollider2D touchingCollider;
    private Animator anim;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] cellingHits = new RaycastHit2D[5];
    [SerializeField] private bool _isGround = false;
    public bool IsGround {
        get { return _isGround; }
        private set
        {
            _isGround = value;
            anim.SetBool(AnimationStrings.isGround, _isGround);
        }
    }
    [SerializeField] private bool _isOnWall = false;
    public bool IsOnWall {
        get { return _isOnWall; }
        private set
        {
            _isOnWall = value;
            anim.SetBool(AnimationStrings.isOnWall, _isOnWall);
        }
    }
    [SerializeField] private bool _isOnCelling = false;
    public bool IsOnCelling {
        get { return _isOnCelling; }
        private set
        {
            _isOnCelling = value;
            anim.SetBool(AnimationStrings.isOnCelling, _isOnCelling);
        }
    }

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    // private Vector2 wallCheckDirection =>GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right;
    
    void Awake()
    {
        touchingCollider = GetComponent<BoxCollider2D> ();
        anim = GetComponent<Animator> ();
    }

    void FixedUpdate()
    {
        IsGround = touchingCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchingCollider.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCelling = touchingCollider.Cast(Vector2.up, castFilter, cellingHits, cellingDistance) > 0;
    }

}
