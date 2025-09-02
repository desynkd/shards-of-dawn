using UnityEngine;
using Photon.Pun;

/// <summary>
/// Setup script to ensure MultiplayerDoor02Trigger has proper Photon networking components
/// This script should be attached to the same GameObject as MultiplayerDoor02Trigger
/// </summary>
public class MultiplayerDoor02Setup : MonoBehaviour
{
    [Header("Setup Configuration")]
    public bool autoSetupPhotonView = true;
    
    private MultiplayerDoor02Trigger doorTrigger;
    private PhotonView photonView;

    void Awake()
    {
        // Get or add the door trigger component
        doorTrigger = GetComponent<MultiplayerDoor02Trigger>();
        if (doorTrigger == null)
        {
            Debug.LogError("[MultiplayerDoor02Setup] MultiplayerDoor02Trigger component not found on GameObject: " + gameObject.name);
            return;
        }

        // Ensure PhotonView component exists
        if (autoSetupPhotonView)
        {
            SetupPhotonView();
        }
    }

    void SetupPhotonView()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
            Debug.Log("[MultiplayerDoor02Setup] Added PhotonView component to: " + gameObject.name);
        }

        // Configure PhotonView settings for door synchronization
        photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
        
        // Add the door trigger script to observed components
        if (!photonView.ObservedComponents.Contains(doorTrigger))
        {
            photonView.ObservedComponents.Add(doorTrigger);
        }
    }

    void Start()
    {
        // Verify setup
        if (doorTrigger != null && photonView != null)
        {
            Debug.Log("[MultiplayerDoor02Setup] GameLevel02 door system setup complete");
        }
        else
        {
            Debug.LogError("[MultiplayerDoor02Setup] Setup incomplete - missing required components");
        }
    }
}

