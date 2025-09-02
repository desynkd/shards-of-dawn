using UnityEngine;
using Photon.Pun;

/// <summary>
/// Simple door trigger system for GameLevel02
/// Handles single player pressure button functionality with multiplayer synchronization
/// </summary>
public class MultiplayerDoor02Trigger : MonoBehaviourPun
{
    [Header("Door and Button References")]
    public GameObject doorToDeactivate; // Assign the door tilemap GameObject here
    public GameObject pressureButton01; // Assign PressureButton01 GameObject
    
    private bool doorDeactivated = false;
    private bool buttonPressed = false;

    void Start()
    {
        // Ensure door starts in active state
        if (doorToDeactivate != null)
        {
            doorToDeactivate.SetActive(true);
        }
        
        Debug.Log("[MultiplayerDoor02Trigger] Initialized for GameLevel02");
    }

    /// <summary>
    /// Called when the pressure button is pressed
    /// </summary>
    public void OnButtonPressed()
    {
        if (doorDeactivated) return;
        
        // Use RPC to synchronize button press across all clients
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_OnButtonPressed", RpcTarget.All);
        }
        else
        {
            // Offline mode - call directly
            HandleButtonPressed();
        }
    }

    [PunRPC]
    private void RPC_OnButtonPressed()
    {
        HandleButtonPressed();
    }

    private void HandleButtonPressed()
    {
        if (doorDeactivated) return;
        
        buttonPressed = true;
        Debug.Log("[MultiplayerDoor02Trigger] Button pressed, deactivating door for all players");
        DeactivateDoor();
    }

    /// <summary>
    /// Called when the pressure button is released
    /// </summary>
    public void OnButtonReleased()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_OnButtonReleased", RpcTarget.All);
        }
        else
        {
            HandleButtonReleased();
        }
    }

    [PunRPC]
    private void RPC_OnButtonReleased()
    {
        HandleButtonReleased();
    }

    private void HandleButtonReleased()
    {
        buttonPressed = false;
        // For GameLevel02, we keep the door deactivated once the button is pressed
        // This creates a permanent door opening mechanic
    }

    private void DeactivateDoor()
    {
        if (doorToDeactivate != null)
        {
            doorToDeactivate.SetActive(false);
            doorDeactivated = true;
            Debug.Log("[MultiplayerDoor02Trigger] Door deactivated for all players");
        }
    }

    /// <summary>
    /// Helper method to check if this is the correct button
    /// </summary>
    public bool IsCorrectButton(GameObject buttonObj)
    {
        return buttonObj == pressureButton01;
    }
}
