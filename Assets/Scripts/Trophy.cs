using System.Collections;
using UnityEngine;

public class Trophy : MonoBehaviour
{
    [SerializeField] private GameObject winScreen; // Canvas with the "You've Won!" text (child of this prefab)
    [SerializeField] private float collectedDuration = 0.4f; // How long the Collected animation plays before the strawberry hides

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool collected;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
