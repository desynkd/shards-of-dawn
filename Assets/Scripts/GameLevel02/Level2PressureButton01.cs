using UnityEngine;
using Photon.Pun;

/// <summary>
/// Pressure Button for GameLevel02
/// Implements single player functionality with multiplayer synchronization
/// </summary>
public class Level2PressureButton01 : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    public GameObject doorToDeactivate; // Assign the door tilemap GameObject in Inspector
    public GameObject doorTrigger; // Assign MultiplayerDoor02Trigger GameObject
    public Sprite pressedSprite; // Assign the pressed sprite in Inspector
    public string playerTag = "Player"; // Tag to identify player objects

    private SpriteRenderer spriteRenderer;
    private bool isPressed = false;
    private MultiplayerDoor02Trigger doorScript;
    private Sprite originalSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Store the original sprite for restoration
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
        
        // Get reference to the door trigger script
        if (doorTrigger != null)
        {
            doorScript = doorTrigger.GetComponent<MultiplayerDoor02Trigger>();
            if (doorScript == null)
            {
                Debug.LogWarning("[Level2PressureButton01] MultiplayerDoor02Trigger component not found on doorTrigger GameObject");
            }
        }
        else
        {
            Debug.LogWarning("[Level2PressureButton01] doorTrigger reference not assigned in Inspector");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if this is a player collision
        if (!isPressed && other.CompareTag(playerTag))
        {
            // Check if this is the local player (client-side validation)
            if (IsLocalPlayer(other.gameObject))
            {
                Debug.Log("[Level2PressureButton01] Local player stepped on pressure button");
                
                // Set button as pressed
                isPressed = true;
                
                // Change sprite to pressed state
                if (spriteRenderer != null && pressedSprite != null)
                {
                    spriteRenderer.sprite = pressedSprite;
                }
                
                // Deactivate door locally for immediate feedback
                if (doorToDeactivate != null)
                {
                    doorToDeactivate.SetActive(false);
                }
                
                // Notify the multiplayer door system to sync across all clients
                if (doorScript != null)
                {
                    doorScript.OnButtonPressed();
                }
                else
                {
                    Debug.LogWarning("[Level2PressureButton01] Door script not found, door deactivation not synchronized");
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Check if this is the local player
            if (IsLocalPlayer(other.gameObject))
            {
                Debug.Log("[Level2PressureButton01] Local player stepped off pressure button");
                
                // Reset button state
                isPressed = false;
                
                // Restore original sprite
                if (spriteRenderer != null && originalSprite != null)
                {
                    spriteRenderer.sprite = originalSprite;
                }
                
                // Notify the door system (though for GameLevel02, door stays deactivated)
                if (doorScript != null)
                {
                    doorScript.OnButtonReleased();
                }
            }
        }
    }

    /// <summary>
    /// Check if the colliding GameObject is the local player
    /// </summary>
    private bool IsLocalPlayer(GameObject playerObj)
    {
        // Check if this is a PhotonView and if it's owned by the local player
        PhotonView playerView = playerObj.GetComponent<PhotonView>();
        if (playerView != null)
        {
            return playerView.IsMine;
        }
        
        // Fallback: if no PhotonView, assume it's the local player in single player mode
        return !PhotonNetwork.InRoom || PhotonNetwork.IsMessageQueueRunning;
    }

    /// <summary>
    /// Public method to manually trigger the button (for testing or external control)
    /// </summary>
    public void TriggerButton()
    {
        if (!isPressed)
        {
            OnTriggerEnter2D(null); // This will trigger the button logic
        }
    }
}

