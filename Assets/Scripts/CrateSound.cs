using UnityEngine;

// Plays a scraping loop while the crate slides along the ground
public class CrateSound : MonoBehaviour
{
    [SerializeField] private float minSpeed = 0.2f; // Horizontal speed needed to count as moving

    private Rigidbody2D rb;
    private AudioSource loop;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (SoundEffects.Instance != null)
        {
            loop = SoundEffects.AddLoop(gameObject, SoundEffects.Instance.crateMoving);
        }
    }

    void Update()
    {
        bool sliding = Mathf.Abs(rb.linearVelocity.x) > minSpeed && Mathf.Abs(rb.linearVelocity.y) < 0.1f;
        SoundEffects.SetLoop(loop, sliding);
    }
}
