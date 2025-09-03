using UnityEngine;

public class GameLevel01MusicManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource gameEndMusicSource;
    
    [Header("Music Settings")]
    public float fadeDuration = 1f;
    
    private static GameLevel01MusicManager instance;
    
    public static GameLevel01MusicManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameLevel01MusicManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Ensure background music is playing and game end music is stopped
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.enabled = true;
            if (!backgroundMusicSource.isPlaying)
            {
                backgroundMusicSource.Play();
            }
        }
        
        if (gameEndMusicSource != null)
        {
            gameEndMusicSource.enabled = false;
            gameEndMusicSource.Stop();
        }
    }
    
    public void SwitchToGameEndMusic()
    {
        Debug.Log("Switching to game end music...");
        
        // Stop background music
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
            backgroundMusicSource.enabled = false;
        }
        
        // Start game end music
        if (gameEndMusicSource != null)
        {
            gameEndMusicSource.enabled = true;
            gameEndMusicSource.Play();
        }
    }
    
    public void SwitchToGameEndMusicWithFade()
    {
        StartCoroutine(SwitchMusicWithFade());
    }
    
    private System.Collections.IEnumerator SwitchMusicWithFade()
    {
        Debug.Log("Switching to game end music with fade...");
        
        // Fade out background music
        if (backgroundMusicSource != null)
        {
            float startVolume = backgroundMusicSource.volume;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
                yield return null;
            }
            
            backgroundMusicSource.volume = 0f;
            backgroundMusicSource.Stop();
            backgroundMusicSource.enabled = false;
        }
        
        // Start game end music and fade it in
        if (gameEndMusicSource != null)
        {
            gameEndMusicSource.enabled = true;
            gameEndMusicSource.volume = 0f;
            gameEndMusicSource.Play();
            
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                gameEndMusicSource.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            
            gameEndMusicSource.volume = 1f;
        }
    }
}
