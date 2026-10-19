using UnityEditor;
using UnityEngine;

// Tools > Setup Sound Effects builds Assets/Resources/SoundEffects.prefab with every clip assigned,
// and adds CrateSound to the Crate prefab. Safe to run again, e.g. after changing a clip below.
public static class SoundEffectsSetup
{
    private const string Sounds = "Assets/SFXPack/Sounds/";
    private const string PrefabPath = "Assets/Resources/SoundEffects.prefab";
    private const string CratePath = "Assets/Prefabs/Crate.prefab";

    [MenuItem("Tools/Setup Sound Effects")]
    public static void Setup()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        GameObject root = new GameObject("SoundEffects");
        SoundEffects fx = root.AddComponent<SoundEffects>();

        fx.jump = Load("Interaction/Jump1.wav");
        fx.land = Load("Weather/SnowLanding.wav");
        fx.footsteps = Load("Interaction/Footsteps.wav");
        fx.trophyAmbient = Load("UI/Chime.wav");
        fx.trophyCollected = Load("UI/Success.wav");
        fx.plateDown = Load("Interaction/Lever.wav");
        fx.plateUp = Load("UI/Click3.wav");
        fx.crateMoving = Load("Weather/IceScrape.wav");
        fx.platformWeak = Load("Weather/IceCrack.wav");
        fx.platformBreak = Load("Interaction/Object Break.wav");
        fx.platformRespawn = Load("Interaction/Power Up.wav");
        fx.platformMoving = Load("Weather/FrozenLatch.wav");

        fx.menuOpen = Load("UI/Menu Open.wav");
        fx.menuClose = Load("UI/Menu Close.wav");
        fx.buttonHover = Load("UI/Tab Switch.wav");
        fx.buttonClick = Load("UI/Button.wav");

        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);

        AddCrateSound();

        AssetDatabase.SaveAssets();
        Debug.Log("Sound effects set up. Prefab saved to " + PrefabPath);
    }

    private static AudioClip Load(string relativePath)
    {
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Sounds + relativePath);
        if (clip == null)
        {
            Debug.LogWarning("Sound effect clip not found: " + Sounds + relativePath);
        }
        return clip;
    }

    private static void AddCrateSound()
    {
        GameObject crate = PrefabUtility.LoadPrefabContents(CratePath);
        if (crate.GetComponent<CrateSound>() == null)
        {
            crate.AddComponent<CrateSound>();
            PrefabUtility.SaveAsPrefabAsset(crate, CratePath);
        }
        PrefabUtility.UnloadPrefabContents(crate);
    }
}
