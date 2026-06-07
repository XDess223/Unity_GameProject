using UnityEngine;

public class ScriptParallaxHandler : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;
    private Vector2 startPos;
    private float startingZ;
    private Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startPos;
    [Range(0.01f, 0.99f)]
    public float parallaxFactor = 0.01f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        startingZ = transform.position.z;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPosition = (startPos + camMoveSinceStart *(-parallaxFactor));

        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
        
    }
}
