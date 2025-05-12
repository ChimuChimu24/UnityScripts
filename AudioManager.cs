using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum SoundEffects
{
    changeSFX,
    placeSFX,
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
 
    public AudioClip resultMusic;
    public AudioClip titleMusic;
    public AudioClip battleMusic;

    [System.Serializable]
    public struct SoundEffectMapping
    {
        public SoundEffects type;
        public AudioClip clip;
    }

    [SerializeField] private List<SoundEffectMapping> soundEffectsList;
    private Dictionary<SoundEffects, AudioClip> soundEffectsDictionary;

    private string currentSceneName = "";
    
    public static AudioManager Instance { get; private set; }


 private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            InitializeSFXSource(); // Ensure SFX source is ready
            PopulateSFXDictionary(); // Build the lookup dictionary

        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentSceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(currentSceneName);
    }

    void InitializeSFXSource()
    {
        if (SFXSource == null)
        {
            Debug.LogWarning("AudioManager: SFXSource not assigned. Trying to find or create.");
            AudioSource[] sources = GetComponents<AudioSource>();
            foreach (AudioSource source in sources)
            {
                if (source != musicSource)
                {
                    SFXSource = source;
                    break;
                }
            }
            if (SFXSource == null) {
                 SFXSource = gameObject.AddComponent<AudioSource>();
                 Debug.Log("AudioManager: Created a new AudioSource for SFX.");
            } else {
                 Debug.Log("AudioManager: Found existing AudioSource for SFX.");
            }
        }

        SFXSource.playOnAwake = false;
        SFXSource.loop = false;
    }

    void PopulateSFXDictionary()
    {
        soundEffectsDictionary = new Dictionary<SoundEffects, AudioClip>();


        foreach (SoundEffectMapping mapping in soundEffectsList)
        {
            if (mapping.clip == null)
            {
                Debug.LogWarning($"AudioManager: AudioClip is null for SFX type '{mapping.type}' in the Inspector list.");
                continue; 
            }

            if (!soundEffectsDictionary.ContainsKey(mapping.type))
            {
                soundEffectsDictionary.Add(mapping.type, mapping.clip);

            }
            else
            {
                Debug.LogWarning($"AudioManager: Duplicate SoundEffects '{mapping.type}' defined in soundEffectsList. Using the first one found.");
            }
        }
         Debug.Log($"AudioManager: Initialized SFX Dictionary with {soundEffectsDictionary.Count} entries.");
    }

    void OnDestroy()
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("AudioManager unsubscribed from sceneLoaded event.");

        if (Instance == this)
        {
            Instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == currentSceneName) return;

        Debug.Log($"Scene loaded: {scene.name}");
        currentSceneName = scene.name; 


        PlayMusicForScene(scene.name);
    }

        void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        if (sceneName == "TitleScecne")
        {
            clipToPlay = titleMusic;
            Debug.Log("Setting music to Battle Music.");
        }
        else if (sceneName == "GameScene") 
        {
            clipToPlay = battleMusic;
            Debug.Log("Setting music to Battle Music.");
        }

        

        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            Debug.Log($"Playing clip: {clipToPlay.name}");
            musicSource.Stop();
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
    }
    
        public void ChangeToResultMusic()
    {
        musicSource.Stop();
        musicSource.clip = resultMusic;
        musicSource.Play();
    }
       public void ChangeTobattleMusic()
    {
        musicSource.Stop();
        musicSource.clip = battleMusic;
        musicSource.Play();
    }

    public void ChangeToTitleMusic()
    {
        musicSource.Stop();
        musicSource.clip = titleMusic;
        musicSource.Play();
    }

 public void PlaySFX(SoundEffects sfxType)
    {
        if (SFXSource == null) {
            Debug.LogError("AudioManager: SFXSource is missing! Cannot play SFX.");
            return;
        }

        if (soundEffectsDictionary.TryGetValue(sfxType, out AudioClip clipToPlay))
        {
            if (clipToPlay != null)
            {
                 SFXSource.PlayOneShot(clipToPlay);
            }
        }
        else
        {
            Debug.LogWarning($"AudioManager: SFX type '{sfxType}' not found in dictionary. Was it added to the soundEffectsList in the Inspector?");
        }
    }
}
