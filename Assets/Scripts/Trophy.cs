using System.Collections;
using UnityEngine;

public class Trophy : MonoBehaviour
{
    [SerializeField] private GameObject winScreen; // Canvas with the "You've Won!" text (child of this prefab)
    [SerializeField] private float collectedDuration = 0.4f; // How long the Collected animation plays before the strawberry hides

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool collected;

    [Header("Ambient sound")]
    [SerializeField] private float ambientInterval = 3f; // Seconds between ambient chimes
    [SerializeField] private float ambientRange = 40f; // Distance from the player at which the chime becomes silent
    [SerializeField] private float ambientVolume = 0.8f; // Chime volume when the player is right next to the trophy

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(AmbientSound());
    }

    // Quietly chime now and then, louder the closer the player is
    IEnumerator AmbientSound()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        while (!collected)
        {
            yield return new WaitForSeconds(ambientInterval);

            if (player != null && !collected && SoundEffects.Instance != null)
            {
                float distance = Vector2.Distance(player.transform.position, transform.position);
                float volume = ambientVolume * Mathf.Clamp01(1f - distance / ambientRange);
                SoundEffects.Play(SoundEffects.Instance.trophyAmbient, volume);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
        {
            return;
        }

        collected = true;
        animator.SetTrigger("Collected");
        winScreen.SetActive(true);

        // Audio keeps playing while the game is frozen with Time.timeScale = 0
        if (SoundEffects.Instance != null)
        {
            SoundEffects.Play(SoundEffects.Instance.trophyCollected);
        }

        // Freeze the game. The Animator uses Unscaled Time so the Collected animation still plays.
        Time.timeScale = 0f;
        StartCoroutine(HideAfterCollected());
    }

    IEnumerator HideAfterCollected()
    {
        // Realtime wait, since normal time is frozen
        yield return new WaitForSecondsRealtime(collectedDuration);
        spriteRenderer.enabled = false;
    }
}
