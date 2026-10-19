using System.Collections;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float breakDelay = 2f; // Time it takes for the platform to break after a player first jumps on it
    [SerializeField] private float respawnDelay = 4f; // Time it takes for the platform to respawn after breaking

    [Header("Detection")]
    [SerializeField] private float topTolerance = 0.1f; // How far below the top edge the player's feet can be and still count as landing on top. Lower this for small objects

    [Header("Visuals")]
    [SerializeField] private Sprite fullSprite; // Full health platform sprite
    [SerializeField] private Sprite weakenedSprite; // Weakened platform sprite

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isTriggered; // True once the player has stepped on the platform and the break timer is running

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // The timer already started, don't start a new timer
        if (isTriggered)
        {
            return;
        }
        // Only the player can make the break timer start
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        // Players can't hit the bottom or side of the platform to trigger the timer
        if (collision.collider.bounds.min.y < platformCollider.bounds.max.y - topTolerance)
        {
            return;
        }

        StartCoroutine(BreakSequence());
    }

    // Function that starts the sequence of the platform breaking 
    // Countdown, weaken at the halfway point, break, then respawn
    private IEnumerator BreakSequence()
    {
        isTriggered = true;

        // Halfway through the timer, swap to the weakened sprite
        yield return new WaitForSeconds(breakDelay / 2f);
        spriteRenderer.sprite = weakenedSprite;
        if (SoundEffects.Instance != null) SoundEffects.Play(SoundEffects.Instance.platformWeak);

        // Second half of the timer, then the platform breaks
        yield return new WaitForSeconds(breakDelay / 2f);
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;
        if (SoundEffects.Instance != null) SoundEffects.Play(SoundEffects.Instance.platformBreak);

        yield return new WaitForSeconds(respawnDelay);

        spriteRenderer.sprite = fullSprite;
        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        if (SoundEffects.Instance != null) SoundEffects.Play(SoundEffects.Instance.platformRespawn);

        isTriggered = false;
    }
}
