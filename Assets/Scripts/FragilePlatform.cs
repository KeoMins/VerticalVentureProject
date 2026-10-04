using System.Collections;
using UnityEngine;

// Attach this to a fragile platform GameObject.
// Requires: SpriteRenderer, a non-trigger Collider2D (e.g. BoxCollider2D).
// The player GameObject must be tagged "Player".
public class FragilePlatform : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 2f;           // seconds of standing on it before it breaks
    [SerializeField] private float respawnDelay = 5f;        // time after breaking before it comes back
    [SerializeField] private float weakenedRevertDelay = 5f; // time spent weakened (undamaged) before healing back to full

    [Header("Visuals")]
    [SerializeField] private Sprite fullSprite;
    [SerializeField] private Sprite weakenedSprite;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private float currentHealth;
    private bool isBroken;
    private bool weakenedShown; // prevents re-assigning the sprite every frame once already weakened
    private float weakenedElapsedTime; // time since the platform was last damaged while weakened

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // Counts up only while weakened and not actively being damaged (that
        // reset happens in OnCollisionStay2D below). If left alone long
        // enough, the platform heals back to full.
        if (!weakenedShown || isBroken) return;

        weakenedElapsedTime += Time.deltaTime;
        if (weakenedElapsedTime >= weakenedRevertDelay)
        {
            RevertToFull();
        }
    }

    // OnCollisionStay2D fires every physics step (FixedUpdate) while the
    // player is in contact, which is exactly what we want for "damage only
    // while actually standing on it."
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isBroken) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // Confirm contact is vertical (top of platform), not a side bump.
        // Using Mathf.Abs here avoids having to guess which sign convention
        // Unity assigns to the normal - we just care that it's mostly
        // vertical rather than horizontal.
        bool playerOnTop = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.y) > 0.5f)
            {
                playerOnTop = true;
                break;
            }
        }
        if (!playerOnTop) return;

        // Use fixedDeltaTime since this callback runs once per physics step.
        currentHealth -= Time.fixedDeltaTime;

        // Being damaged counts as "in use," so the revert-to-full countdown
        // shouldn't progress right now.
        weakenedElapsedTime = 0f;

        if (!weakenedShown && currentHealth <= maxHealth / 2f)
        {
            spriteRenderer.sprite = weakenedSprite;
            weakenedShown = true;
            // TODO: play "getting weak" sound here
        }

        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    private void RevertToFull()
    {
        currentHealth = maxHealth;
        weakenedShown = false;
        weakenedElapsedTime = 0f;
        spriteRenderer.sprite = fullSprite;
        // TODO: play "reset" sound here
    }

    private void Break()
    {
        isBroken = true;
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;
        // TODO: play "breaking" sound here
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        currentHealth = maxHealth;
        weakenedShown = false;
        weakenedElapsedTime = 0f;
        spriteRenderer.sprite = fullSprite;
        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        // TODO: play "reset" sound here

        isBroken = false;
    }
}