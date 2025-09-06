using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Victory Message")]
    public string victoryMessage = "Congratulations! You have completed Shards of Dawn!";
    public float victoryTextDisplayTime = 3f;

    [Header("Final Level Effects")]
    public ParticleSystem celebrationParticles; // Assign in inspector
    public AudioSource victorySound; // Assign in inspector

    private HashSet<int> playersWhoTouched = new HashSet<int>();
    private bool gameEnding = false;
    private GameEndManager3 gameEndManager;
    private BoxCollider2D triggerCollider;

    void Start()
    {
        // Find or create the GameEndManager3
        gameEndManager = FindFirstObjectByType<GameEndManager3>();
        if (gameEndManager == null)
        {
            GameObject managerGO = new GameObject("GameEndManager3");
            gameEndManager = managerGO.AddComponent<GameEndManager3>();
        }

        // Setup trigger collider
        SetupTriggerCollider();

        // Setup celebration effects
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
            StartGameEndSequence();
        }
    }

    void StartGameEndSequence()
    {
        if (gameEnding) return;

        gameEnding = true;
        Debug.Log("All players have reached the final crystal! Starting game end sequence...");

        // Play celebration effects
        PlayCelebrationEffects();

        // Start the game end sequence
        gameEndManager.StartGameEndSequence(
            endCameraPosition,
            endCameraZoom,
            cameraMoveSpeed,
            cameraZoomSpeed,
            visionRevealSpeed,
            whiteFadeSpeed,
            sceneTransitionDelay,
            victoryMessage,
            victoryTextDisplayTime
        );
    }

    void PlayCelebrationEffects()
    {
        // Play celebration particles
        if (celebrationParticles != null)
        {
            celebrationParticles.Play();
        }

        // Play victory sound
        if (victorySound != null)
        {
            victorySound.Play();
        }
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
            StartGameEndSequence();
        }
    }
}
