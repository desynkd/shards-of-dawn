using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Reflection;

public class GameEndCrystal3 : MonoBehaviour
{
    [Header("Final Game End Settings")]
    public Vector2 endCameraPosition = new Vector2(0, 0);
    public float endCameraZoom = 12f; // Slightly more dramatic zoom for final level
    public float cameraMoveSpeed = 6f; // Slower for more dramatic effect
    public float cameraZoomSpeed = 3f; // Slower zoom for cinematic feel
    public float visionRevealSpeed = 0.15f; // Slower reveal for drama
    public float whiteFadeSpeed = 0.2f; // Slower fade for final effect
    public float sceneTransitionDelay = 2f; // Longer delay for final celebration

    [Header("Crystal Settings")]
    public bool isTrigger = true;
    public Vector2 triggerSize = new Vector2(1f, 1f);

    [Header("Final Level Effects")]
    public ParticleSystem celebrationParticles; // Assign in inspector
    public AudioSource victorySound; // Assign in inspector
    public float particleDuration = 3f;
    public float soundFadeOutDuration = 1f;

    [Header("Victory Text")]
    public string victoryMessage = "Congratulations! You have completed Shards of Dawn!";
    public float victoryTextDisplayTime = 3f;

    private HashSet<int> playersWhoTouched = new HashSet<int>();
    private bool gameEnding = false;
    private MonoBehaviour gameEndManager;
    private BoxCollider2D triggerCollider;

    void Start()
    {
        // Find or create the GameEndManager3
        var existingManager = FindFirstObjectByType<MonoBehaviour>();
        if (existingManager != null && existingManager.GetType().Name == "GameEndManager3")
        {
            gameEndManager = existingManager;
        }
        else
        {
            GameObject managerGO = new GameObject("GameEndManager3");
            // Use reflection to add the component
            System.Type managerType = System.Type.GetType("GameEndManager3");
            if (managerType != null)
            {
                gameEndManager = (MonoBehaviour)managerGO.AddComponent(managerType);
            }
            else
            {
                Debug.LogError("GameEndManager3 type not found! Make sure the script exists and compiles correctly.");
            }
        }

        // Setup trigger collider
        SetupTriggerCollider();

        // Setup particle system if assigned
        if (celebrationParticles != null)
        {
            celebrationParticles.Stop();
        }
    }

    void SetupTriggerCollider()
    {
        // Add or get BoxCollider2D
        triggerCollider = GetComponent<BoxCollider2D>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Configure as trigger
        triggerCollider.isTrigger = isTrigger;
        triggerCollider.size = triggerSize;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameEnding) return;

        Player player = other.GetComponent<Player>();
        if (player == null || player.pv == null) return;

        // Add player to the list of those who touched the crystal
        playersWhoTouched.Add(player.pv.OwnerActorNr);

        Debug.Log($"Player {player.pv.OwnerActorNr} reached the final crystal! ({playersWhoTouched.Count} players total)");

        // Check if all players have touched the crystal
        CheckGameEnd();
    }

    void CheckGameEnd()
    {
        int requiredPlayers = 1; // Default for single player/developer mode

        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
        {
            requiredPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        }

        if (playersWhoTouched.Count >= requiredPlayers)
        {
            StartFinalGameEndSequence();
        }
    }

    void StartFinalGameEndSequence()
    {
        if (gameEnding) return;

        gameEnding = true;
        Debug.Log("🎉 ALL PLAYERS HAVE COMPLETED SHARDS OF DAWN! 🎉");

        // Start celebration effects immediately
        StartCelebrationEffects();

        // Start the final game end sequence using reflection
        if (gameEndManager != null)
        {
            MethodInfo method = gameEndManager.GetType().GetMethod("StartFinalGameEndSequence");
            if (method != null)
            {
                object[] parameters = new object[]
                {
                    endCameraPosition,
                    endCameraZoom,
                    cameraMoveSpeed,
                    cameraZoomSpeed,
                    visionRevealSpeed,
                    whiteFadeSpeed,
                    sceneTransitionDelay,
                    victoryMessage,
                    victoryTextDisplayTime
                };
                method.Invoke(gameEndManager, parameters);
            }
            else
            {
                Debug.LogError("StartFinalGameEndSequence method not found on GameEndManager3!");
            }
        }
        else
        {
            Debug.LogError("GameEndManager not found!");
        }
    }

    void StartCelebrationEffects()
    {
        // Play celebration particles
        if (celebrationParticles != null)
        {
            celebrationParticles.Play();
            StartCoroutine(StopParticlesAfterDelay());
        }

        // Play victory sound
        if (victorySound != null)
        {
            victorySound.Play();
            StartCoroutine(FadeOutSound());
        }

        // You could add more effects here like:
        // - Screen shake
        // - Color cycling
        // - Crystal glow animation
        // - Fireworks
    }

    IEnumerator StopParticlesAfterDelay()
    {
        yield return new WaitForSeconds(particleDuration);
        if (celebrationParticles != null)
        {
            celebrationParticles.Stop();
        }
    }

    IEnumerator FadeOutSound()
    {
        if (victorySound == null) yield break;

        float startVolume = victorySound.volume;
        float elapsed = 0f;

        while (elapsed < soundFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            victorySound.volume = Mathf.Lerp(startVolume, 0f, elapsed / soundFadeOutDuration);
            yield return null;
        }

        victorySound.Stop();
        victorySound.volume = startVolume; // Reset for next time
    }

    // Visual debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; // Different color for final crystal
        Gizmos.DrawWireCube(transform.position, triggerSize);

        // Draw a larger outer glow effect for final crystal
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireCube(transform.position, triggerSize * 1.5f);
    }

    // Public method to manually trigger the end sequence (for testing)
    [ContextMenu("Trigger Final Game End")]
    public void TriggerFinalEnd()
    {
        if (!gameEnding)
        {
            playersWhoTouched.Clear();
            playersWhoTouched.Add(1); // Add a dummy player
            StartFinalGameEndSequence();
        }
    }
}
