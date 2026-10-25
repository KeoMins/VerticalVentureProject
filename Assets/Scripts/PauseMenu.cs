using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Pause menu opened with Esc: Resume, Restart and Exit
public class PauseMenu : MonoBehaviour
{
    private GameObject panel;
    private bool isPaused;
    private float previousTimeScale = 1f; // Restored on resume

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        new GameObject("PauseMenu").AddComponent<PauseMenu>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Survives Restart, which reloads the scene
        BuildUI();
        EnsureEventSystem();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureEventSystem();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        isPaused = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        AudioListener.pause = true; // Silences the game's sounds, menu sounds ignore this

        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); // Clear leftovers from earlier clicks so no button starts highlighted
        PlaySound(SoundEffects.Instance != null ? SoundEffects.Instance.menuOpen : null);
    }

    private void Resume()
    {
        Unfreeze();
        PlaySound(SoundEffects.Instance != null ? SoundEffects.Instance.menuClose : null);
    }

    private void Restart()
    {
        PlaySound(SoundEffects.Instance != null ? SoundEffects.Instance.buttonClick : null);

        // Reloading the scene puts everything, including the player, back at the start
        isPaused = false;
        panel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Exit()
    {
        PlaySound(SoundEffects.Instance != null ? SoundEffects.Instance.buttonClick : null);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Unfreeze()
    {
        isPaused = false;
        panel.SetActive(false);
        Time.timeScale = previousTimeScale;
        AudioListener.pause = false;
    }

    private static void PlaySound(AudioClip clip)
    {
        SoundEffects.Play(clip);
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    // ---- UI construction ----

    private void BuildUI()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject canvasObject = new GameObject("PauseMenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Above the trophy's win screen

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        // Dark overlay covering the whole screen
        panel = CreateChild("Panel", canvasObject.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);

        CreateText("Title", "Paused", panel.transform, font, 110, new Vector2(0f, 300f), new Vector2(800f, 160f));

        CreateButton("Resume", panel.transform, font, new Vector2(0f, 80f), Resume);
        CreateButton("Restart", panel.transform, font, new Vector2(0f, -70f), Restart);
        CreateButton("Exit", panel.transform, font, new Vector2(0f, -220f), Exit);

        panel.SetActive(false);
    }

    private static GameObject CreateChild(string name, Transform parent)
    {
        GameObject child = new GameObject(name, typeof(RectTransform));
        child.transform.SetParent(parent, false);
        return child;
    }

    private static Text CreateText(string name, string content, Transform parent, Font font, int size, Vector2 position, Vector2 dimensions)
    {
        GameObject textObject = CreateChild(name, parent);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = dimensions;

        Text text = textObject.AddComponent<Text>();
        text.text = content;
        text.font = font;
        text.fontSize = size;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private Button CreateButton(string label, Transform parent, Font font, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = CreateChild(label + " Button", parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(480f, 110f);

        Image background = buttonObject.AddComponent<Image>();
        background.color = Color.white; // The button's colour tint is applied on top of this

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = background;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.18f, 0.18f, 0.24f, 1f);
        colors.highlightedColor = new Color(0.35f, 0.35f, 0.55f, 1f);
        colors.selectedColor = colors.normalColor; // A clicked button stays "selected", so don't tint it
        colors.pressedColor = new Color(0.5f, 0.5f, 0.75f, 1f);
        button.colors = colors;
        button.onClick.AddListener(onClick);

        buttonObject.AddComponent<HoverSound>();

        CreateText("Label", label, buttonObject.transform, font, 56, Vector2.zero, rect.sizeDelta);
        return button;
    }

    // Plays the hover sound when the mouse moves onto a button
    private class HoverSound : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (SoundEffects.Instance != null)
            {
                SoundEffects.Play(SoundEffects.Instance.buttonHover);
            }
        }
    }
}
