using UnityEngine;
using Photon.Pun;

public class YellowPressureButton : MonoBehaviour
{
    [Header("Pressure Button Settings")]
    public YellowLauncherTilemap targetLauncher; // Assign the YellowLauncherTilemap in Inspector
    public Sprite pressedSprite; // Assign the pressed sprite in Inspector
    public string playerTag = "Player"; // Tag to identify player objects

    private SpriteRenderer spriteRenderer;
    private bool isPressed = false;
    private bool isPlayerOnButton = false;
    private float temporaryDeactivationTimer = 0f;
    private float temporaryDeactivationDuration = 3f; // Duration for temporary deactivation in 4+ player mode

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Initialize button state based on player count
        UpdateButtonState();
    }

    void Update()
    {
        // Handle temporary deactivation timer for 4+ player mode
        if (temporaryDeactivationTimer > 0)
        {
            temporaryDeactivationTimer -= Time.deltaTime;
            if (temporaryDeactivationTimer <= 0)
            {
                // Reactivate the launcher when timer expires
                if (targetLauncher != null)
                {
                    targetLauncher.SetLauncherActive(true);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerOnButton = true;
            HandleButtonPress();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerOnButton = false;
            // For 4+ player mode, start timer when player leaves button
            if (GetPlayerCount() >= 4)
            {
                temporaryDeactivationTimer = temporaryDeactivationDuration;
            }
        }
    }

    void HandleButtonPress()
    {
        int playerCount = GetPlayerCount();

        // Check if button should be active based on player count
        if (playerCount < 2)
        {
            // For testing purposes, allow button press when player count is 0 (offline mode)
            if (playerCount == 0)
            {
                // Allow button press for testing
                Debug.Log("[YellowPressureButton] Button press allowed for testing (offline mode)");
            }
            else
            {
                // Button inactive for 1 player (online single player)
                return;
            }
        }

        // Button is active for 2+ players
        if (!isPressed)
        {
            isPressed = true;
            
            // Change sprite to pressed state
            if (spriteRenderer != null && pressedSprite != null)
            {
                spriteRenderer.sprite = pressedSprite;
            }

            // Handle launcher deactivation based on player count
            if (targetLauncher != null)
            {
                if (playerCount == 3)
                {
                    // Permanent deactivation for 3 players
                    targetLauncher.SetLauncherActive(false);
                    Debug.Log("[YellowPressureButton] Launcher permanently deactivated (3 players)");
                }
                else if (playerCount >= 4)
                {
                    // Temporary deactivation for 4+ players
                    targetLauncher.SetLauncherActive(false);
                    temporaryDeactivationTimer = temporaryDeactivationDuration;
                    Debug.Log("[YellowPressureButton] Launcher temporarily deactivated (4+ players)");
                }
            }
        }
    }

    int GetPlayerCount()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
        {
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }
        return 0; // Not in a room, treat as developer/offline mode
    }

    void UpdateButtonState()
    {
        int playerCount = GetPlayerCount();
        
        // Set initial state based on player count
        if (playerCount < 2)
        {
            // For testing purposes, activate button and launcher when player count is 0 (offline mode)
            if (playerCount == 0)
            {
                if (targetLauncher != null)
                {
                    targetLauncher.SetLauncherActive(true);
                }
                Debug.Log("[YellowPressureButton] Button and launcher active for testing (offline mode)");
            }
            else
            {
                // Button and launcher inactive for 1 player (online single player)
                if (targetLauncher != null)
                {
                    targetLauncher.SetLauncherActive(false);
                }
                Debug.Log("[YellowPressureButton] Button and launcher inactive (player count = 1)");
            }
        }
        else
        {
            // Button and launcher active for 2+ players
            if (targetLauncher != null)
            {
                targetLauncher.SetLauncherActive(true);
            }
            Debug.Log("[YellowPressureButton] Button and launcher active (player count >= 2)");
        }
    }

    // Public method to reset button state (useful for testing or level reset)
    public void ResetButton()
    {
        isPressed = false;
        isPlayerOnButton = false;
        temporaryDeactivationTimer = 0f;
        
        // Reset sprite to original
        if (spriteRenderer != null)
        {
            // You might want to store the original sprite in a variable
            // For now, this will need to be handled in the inspector
        }
        
        UpdateButtonState();
    }
}
