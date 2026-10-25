using UnityEngine;

// Plays the game's sound effects
public class SoundEffects : MonoBehaviour
{
    public static SoundEffects Instance { get; private set; }

    [Header("Player")]
    public AudioClip jump;
    public AudioClip land;
    public AudioClip footsteps; // Loops while the player walks

    [Header("Trophy")]
    public AudioClip trophyAmbient;
    public AudioClip trophyCollected;

    [Header("Pressure plate")]
    public AudioClip plateDown;
    public AudioClip plateUp;

    [Header("Crate")]
    public AudioClip crateMoving; // Loops while the crate slides

    [Header("Fragile platform")]
    public AudioClip platformWeak;
    public AudioClip platformBreak;
    public AudioClip platformRespawn;

    [Header("Moving platform")]
    public AudioClip platformMoving; // Loops while the platform moves

    [Header("Pause menu")]
    public AudioClip menuOpen;
    public AudioClip menuClose;
    public AudioClip buttonHover;
    public AudioClip buttonClick;

    [Range(0f, 1f)] public float masterVolume = 1f;

    private AudioSource oneShotSource;

    // Create the manager from the prefab in Resources before the first scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        GameObject prefab = Resources.Load<GameObject>("SoundEffects");
        if (prefab == null)
        {
            Debug.LogWarning("SoundEffects prefab not found in Resources. Run Tools > Setup Sound Effects.");
            return;
        }

        Instantiate(prefab);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.playOnAwake = false;
        oneShotSource.ignoreListenerPause = true; // Menu sounds must play while the game is paused
    }

    // Play a clip once. Safe to call when the manager or clip is missing
    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (Instance == null || clip == null)
        {
            return;
        }

        Instance.oneShotSource.PlayOneShot(clip, volume * Instance.masterVolume);
    }

    // Add a looping AudioSource to an object. Start and stop it with SetLoop
    public static AudioSource AddLoop(GameObject target, AudioClip clip)
    {
        AudioSource source = target.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        return source;
    }

    public static void SetLoop(AudioSource source, bool shouldPlay, float volume = 1f)
    {
        if (source == null || source.clip == null)
        {
            return;
        }

        source.volume = volume * (Instance != null ? Instance.masterVolume : 1f);

        if (shouldPlay && !source.isPlaying)
        {
            source.Play();
        }
        else if (!shouldPlay && source.isPlaying)
        {
            source.Stop();
        }
    }
}
