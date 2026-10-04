using System.Collections;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 2f; // How long the player can stand on the platform before it breaks
    [SerializeField] private float respawnDelay = 5f; // Time it takes for the platform to respawn after breaking
    [SerializeField] private float weakenedRevertDelay = 5f; // Time it takes for the weakened platform to go back to full health

    [Header("Visuals")]
    [SerializeField] private Sprite fullSprite; // Full health platform sprite
    [SerializeField] private Sprite weakenedSprite; // Weakened platform sprite

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private float currentHealth; // Platform's current health
    private bool isBroken; // if the platform is broken or not
    private bool weakenedShown; // Prevents re-assigning the sprite every frame once already weakened
    private float weakenedElapsedTime; // Time since the platform was last damaged while weakened

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // If the platform is full health or already broken, don't try to revert it to full health
        if (!weakenedShown || isBroken)
        {
            return;
        }

        // Increased the time the platform has been in the weakened stage
        weakenedElapsedTime += Time.deltaTime;

        // Weakened platform reverts back to full health if it is left alone longer than the weakenedRevertDelay threshold
        if (weakenedElapsedTime >= weakenedRevertDelay)
        {
            RevertToFull();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // If the platform is broke, don't do anything
        if (isBroken)
        {
            return;
        }
        // Only the player should be able to wear down the platform
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        // Make sure the player is on top of the platform, rather than hitting the side
        bool playerOnTop = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.y) > 0.5f)
            {
                playerOnTop = true;
                break;
            }
        }
        // If the player hits the platform on the side, it doesn't damage the platform
        if (!playerOnTop)
        {
            return;
        }

        // Decreased the health of the platform based on the time the player has been on it
        currentHealth -= Time.fixedDeltaTime;

        // Being damaged counts as "in use," so the revert-to-full countdown
        // shouldn't progress right now.
        weakenedElapsedTime = 0f;

        // Swap to the weakened platform sprite when its health drops below half
        if (!weakenedShown && currentHealth <= maxHealth / 2f)
        {
            spriteRenderer.sprite = weakenedSprite;
            weakenedShown = true;
            // TODO: play "getting weak" sound here
        }
        // Break the platform if it is out of health
        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    // Function to revert weakened platform back to full health
    private void RevertToFull()
    {
        currentHealth = maxHealth;
        weakenedShown = false;
        weakenedElapsedTime = 0f;
        spriteRenderer.sprite = fullSprite;
        // TODO: play "reset" sound here
    }

    // Function to make weakened platform break & disappear
    private void Break()
    {
        isBroken = true;
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;
        // TODO: play "breaking" sound here
        StartCoroutine(RespawnAfterDelay());
    }

    // Funcction to make platform respawn after being broken
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