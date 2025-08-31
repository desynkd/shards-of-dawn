using UnityEngine;

public class LogoSoundController : MonoBehaviour
{
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // TEST: Play sound after 2 seconds
        Invoke("PlayLogoSound", 2f);
    }
    
    public void PlayLogoSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Debug.Log("Playing logo sound!");
        }
        else
        {
            Debug.Log("Audio Source or Clip is missing!");
        }
    }
}