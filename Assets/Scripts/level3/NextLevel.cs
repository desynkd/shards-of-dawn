using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;

public class NextLevel : MonoBehaviour
{
    [Header("Transition Settings")]
    public float fadeSpeed = 0.5f; // Speed of the fade transition
    public float transitionDelay = 0.2f; // Brief delay before scene change

    private HashSet<int> playersWhoReached = new HashSet<int>();
    private bool levelTransitioning = false;

    // UI elements for transition fade
    private Canvas fadeCanvas;
    private UnityEngine.UI.Image fadeImage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelTransitioning) return;

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player == null || player.pv == null) return;

            // Add player to the list of those who reached the exit
            playersWhoReached.Add(player.pv.OwnerActorNr);

            Debug.Log($"Player {player.pv.OwnerActorNr} reached the exit! ({playersWhoReached.Count} players total)");

            // Check if we should advance to next level
            CheckLevelAdvance();
        }
    }

    private void CheckLevelAdvance()
    {
        int requiredPlayers = 1; // Default for single player/developer mode

        // Check Photon player count
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
        {
            requiredPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        }

        // If Photon player count is null, 0, or 1, use current logic (immediate transition)
        // If more than 1 player, wait for all players to reach the exit
        if (requiredPlayers <= 1 || playersWhoReached.Count >= requiredPlayers)
        {
            AdvanceToNextLevel();
        }
        else
        {
            Debug.Log($"Waiting for more players to reach the exit. {playersWhoReached.Count}/{requiredPlayers} players ready.");
        }
    }

    private void AdvanceToNextLevel()
    {
        if (levelTransitioning) return;

        levelTransitioning = true;
        Debug.Log("All required players have reached the exit! Starting door transition...");

        // Start the door transition sequence
        StartCoroutine(DoorTransitionSequence());
    }

    private IEnumerator DoorTransitionSequence()
    {
        Debug.Log("Starting door transition...");

        // Start fade to black
        yield return StartCoroutine(FadeToBlack(fadeSpeed));

        // Brief delay while screen is black
        yield return new WaitForSeconds(transitionDelay);

        // Load next scene
        LoadNextScene();
    }

    private IEnumerator FadeToBlack(float speed)
    {
        Debug.Log("Fading to black...");

        // Create fade canvas
        CreateFadeCanvas();

        float elapsedTime = 0f;
        Color startColor = new Color(0f, 0f, 0f, 0f); // Transparent black
        Color endColor = new Color(0f, 0f, 0f, 1f);   // Opaque black

        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / speed;
            fadeImage.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        // Ensure fully black
        fadeImage.color = endColor;
        Debug.Log("Fade to black completed");
    }

    private void CreateFadeCanvas()
    {
        // Create canvas
        GameObject canvasGO = new GameObject("DoorTransitionCanvas");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999; // Ensure it's on top
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Create fade image
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        fadeImage = imageGO.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f); // Start transparent

        // Set image to fill screen
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void LoadNextScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string nextScene = "";

        if (currentScene == "Level03-1")
        {
            nextScene = "Level03-2";
        }
        else if (currentScene == "Level03-2")
        {
            nextScene = "Level03-3";
        }

        if (!string.IsNullOrEmpty(nextScene))
        {
            Debug.Log($"Loading next scene: {nextScene}");

            // Load scene based on whether we're in a Photon room
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(nextScene);
                }
            }
            else
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (levelTransitioning) return;

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player == null || player.pv == null) return;

            // Remove player from the list if they leave the exit area
            playersWhoReached.Remove(player.pv.OwnerActorNr);

            Debug.Log($"Player {player.pv.OwnerActorNr} left the exit area. ({playersWhoReached.Count} players remaining)");
        }
    }

    // Visual debugging
    private void OnDrawGizmosSelected()
    {
        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        if (boxCol != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (Vector3)boxCol.offset, boxCol.size);

            // Draw larger area to show effect range
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position + (Vector3)boxCol.offset, boxCol.size);
        }
    }

    // Public method to manually trigger level advance (for testing)
    [ContextMenu("Force Level Advance")]
    public void ForceAdvance()
    {
        if (!levelTransitioning)
        {
            playersWhoReached.Clear();
            playersWhoReached.Add(1); // Add a dummy player
            StartCoroutine(DoorTransitionSequence());
        }
    }
}
