using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class GameEndCrystal2 : MonoBehaviour
{
    [Header("Game End Settings")]
    public Vector2 endCameraPosition = new Vector2(0, 0);
    public float endCameraZoom = 10f;
    public float cameraMoveSpeed = 2f;
    public float cameraZoomSpeed = 1f;
    public float visionRevealSpeed = 1f;
    public float whiteFadeSpeed = 1f;
    public float sceneTransitionDelay = 2f;

    [Header("Crystal Settings")]
    public bool isTrigger = true;
    public Vector2 triggerSize = new Vector2(1f, 1f);

    private HashSet<int> playersWhoTouched = new HashSet<int>();
    private bool gameEnding = false;
    private GameEndManager2 gameEndManager;
    private BoxCollider2D triggerCollider;

    void Start()
    {
        // Find or create the GameEndManager2
        gameEndManager = FindFirstObjectByType<GameEndManager2>();
        if (gameEndManager == null)
        {
            GameObject managerGO = new GameObject("GameEndManager2");
            gameEndManager = managerGO.AddComponent<GameEndManager2>();
        }

        // Setup trigger collider
        SetupTriggerCollider();
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
        Debug.Log("All players have reached the crystal! Starting game end sequence...");
        
        // Start the game end sequence
        gameEndManager.StartGameEndSequence(endCameraPosition, endCameraZoom, cameraMoveSpeed, cameraZoomSpeed, visionRevealSpeed, whiteFadeSpeed, sceneTransitionDelay);
    }

    // Visual debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, triggerSize);
    }
}
