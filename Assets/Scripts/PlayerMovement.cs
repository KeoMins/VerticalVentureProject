using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;      // Set this to "Ground" in the Inspector
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private float horizontalInput;
    private float lastHorizontalKey;
    private bool isGrounded;

    private void Awake()
    {
        // Cache component references once instead of calling GetComponent every frame.
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Read input in Update (input polling should not be tied to the physics step).
        // When both keys are held, the most recently pressed one wins.
        if (Input.GetKeyDown(KeyCode.A)) lastHorizontalKey = -1f;
        if (Input.GetKeyDown(KeyCode.D)) lastHorizontalKey = 1f;

        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        if (left && right) horizontalInput = lastHorizontalKey;
        else if (left) horizontalInput = -1f;
        else if (right) horizontalInput = 1f;
        else horizontalInput = 0f;

        // Recheck ground state every frame so jump input feels responsive.
        isGrounded = CheckGrounded();

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        // All physics-affecting movement happens in FixedUpdate for consistent behavior
        // regardless of frame rate.
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        // Reset vertical velocity first so jump height is consistent even if the
        // player is already moving upward/downward slightly (e.g. off a slope).
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool CheckGrounded()
    {
        // Casts a short box downward from the collider's bottom edge to detect ground.
        // Using a box (not a single raycast) avoids false negatives at the edges of platforms.
        Bounds bounds = col.bounds;
        Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, 0.05f); // slightly narrower than the collider
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            boxSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    // Visualizes the ground check box in the Scene view for debugging.
    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Bounds bounds = col.bounds;
        Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, 0.05f);
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - groundCheckDistance / 2f);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(origin, boxSize);
    }
}