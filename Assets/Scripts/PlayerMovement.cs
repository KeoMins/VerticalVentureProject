using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // How fast the player moves
    [SerializeField] private float jumpForce = 10f; // How high the player jumps
    [SerializeField] private LayerMask groundLayer; // What counts as ground

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;
    private float lastKey;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Remember which key was pressed last, for when both are held
        if (Input.GetKeyDown(KeyCode.A))
        {
            lastKey = -1f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastKey = 1f;
        }

        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        if (left && right)
        {
            horizontalInput = lastKey;
        }
        else if (left)
        {
            horizontalInput = -1f;
        }
        else if (right)
        {
            horizontalInput = 1f;
        }
        else
        {
            horizontalInput = 0f;
        }

        // Check if the player is on the ground
        Vector2 boxCenter = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Vector2 boxSize = new Vector2(col.bounds.size.x * 0.9f, 0.05f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0.1f, groundLayer);
        isGrounded = hit.collider != null;

        // Only jump if the player is on the ground
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Face the direction of movement (sprites face right by default)
        if (horizontalInput != 0f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }

        // Drive the animator
        animator.SetFloat("speed", Mathf.Abs(horizontalInput));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
}
