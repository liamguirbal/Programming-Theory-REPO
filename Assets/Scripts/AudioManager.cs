using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Musique - Playlist")]
    public AudioClip[] backgroundMusicPlaylist;
    private int currentMusicIndex = 0;

    [Range(0f, 1f)]
    public float musicVolume = 0.7f;

    [Header("Effets Sonores")]
    public AudioClip powerUpPickupSFX;
    public AudioClip shieldBlockSFX;
    public AudioClip deathSFX;
    public AudioClip scorePointSFX;

    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;

    [Header("Paramètres")]
    [Tooltip("La musique persiste entre les scènes")]
    public bool persistBetweenScenes = true;

    void Awake()
    {
        // Singleton pattern avec persistence
        if (Instance == null)
        {
            Instance = this;

            // Faire persister cet objet entre les scènes
            if (persistBetweenScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
           
            Destroy(gameObject);
            return;
        }

        // Créer  AudioSources
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = false;
        musicSource.volume = musicVolume;

        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
      
        if (!musicSource.isPlaying)
        {
            PlayNextMusic();
        }

        Debug.Log($"AudioManager démarré dans la scène : {SceneManager.GetActiveScene().name}");
    }

    void Update()
    {
        if (!musicSource.isPlaying && backgroundMusicPlaylist.Length > 0)
        {
            PlayNextMusic();
        }
    }

    public void PlayNextMusic()
    {
        if (backgroundMusicPlaylist == null || backgroundMusicPlaylist.Length == 0)
        {
            Debug.LogWarning("Aucune musique dans la playlist !");
            return;
        }

        // Choisir  musique aléatoire
        int randomIndex = Random.Range(0, backgroundMusicPlaylist.Length);

        // S'assurer qu'on ne joue pas la même musique deux fois d'affilée
        while (randomIndex == currentMusicIndex && backgroundMusicPlaylist.Length > 1)
        {
            randomIndex = Random.Range(0, backgroundMusicPlaylist.Length);
        }

        currentMusicIndex = randomIndex;
        AudioClip nextMusic = backgroundMusicPlaylist[currentMusicIndex];

        if (nextMusic != null)
        {
            musicSource.clip = nextMusic;
            musicSource.Play();
            Debug.Log($"Musique lancée : {nextMusic.name} (Scène: {SceneManager.GetActiveScene().name})");
        }
    }

    public void SkipToNextMusic()
    {
        PlayNextMusic();
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    //  Reprendre la musique si elle a été stoppée
    public void ResumeBackgroundMusic()
    {
        if (!musicSource.isPlaying && musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip sfxClip, float volumeScale = 1f)
    {
        if (sfxClip != null)
        {
            sfxSource.PlayOneShot(sfxClip, sfxVolume * volumeScale);
        }
    }

    public void PlayPowerUpPickup()
    {
        PlaySFX(powerUpPickupSFX);
    }

    public void PlayShieldBlock()
    {
        PlaySFX(shieldBlockSFX);
    }

    public void PlayDeath()
    {
        PlaySFX(deathSFX, 1.2f);
    }

    public void PlayScorePoint()
    {
        PlaySFX(scorePointSFX, 0.5f);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }
}
