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

    // Sound effects
    [SerializeField] private float minAirTimeForLanding = 0.1f; // Shortest fall that plays the landing sound, so ground flicker stays silent
    private AudioSource footstepLoop;
    private bool wasGrounded;
    private float airTime;
    private float lastJumpSoundTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (SoundEffects.Instance != null)
        {
            footstepLoop = SoundEffects.AddLoop(gameObject, SoundEffects.Instance.footsteps);
        }
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

        // Play the landing sound when touching the ground after a real fall or jump
        if (!isGrounded)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            if (!wasGrounded && airTime >= minAirTimeForLanding && SoundEffects.Instance != null)
            {
                SoundEffects.Play(SoundEffects.Instance.land);
            }
            airTime = 0f;
        }
        wasGrounded = isGrounded;

        // Footsteps loop only while walking on the ground
        SoundEffects.SetLoop(footstepLoop, horizontalInput != 0f && isGrounded);

        // Only jump if the player is on the ground
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // The ground check stays true for a few frames after leaving the ground, so limit the sound to once per jump
            if (Time.time - lastJumpSoundTime > 0.3f && SoundEffects.Instance != null)
            {
                SoundEffects.Play(SoundEffects.Instance.jump);
                lastJumpSoundTime = Time.time;
            }
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
