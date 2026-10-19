using UnityEngine;

// Puts the crate back where it started if it touches the floor, so a crate pushed off a ledge can be recovered
public class CrateRespawn : MonoBehaviour
{
    [SerializeField] private string floorNamePrefix = "Floor"; // Objects whose name starts with this count as the floor

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float startRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        startRotation = rb.rotation;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.name.StartsWith(floorNamePrefix))
        {
            return;
        }

        // Back to the start, and stop all movement so it doesn't keep sliding or spinning
        rb.position = startPosition;
        rb.rotation = startRotation;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
