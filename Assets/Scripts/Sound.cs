using UnityEngine;

public class Sound : MonoBehaviour
{
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            Debug.Log("No Audio Source found!");
            return;
        }
        
        if (audioSource.clip == null)
        {
            Debug.Log("No Audio Clip assigned!");
            return;
        }
        
        Debug.Log("Playing audio: " + audioSource.clip.name);
        audioSource.volume = 1f;
        audioSource.Play();
        
        // Check if it's actually playing
        if (audioSource.isPlaying)
        {
            Debug.Log("Audio is playing!");
        }
        else
        {
            Debug.Log("Audio failed to play!");
        }
    }
}