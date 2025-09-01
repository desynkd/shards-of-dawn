using UnityEngine;
using Photon.Pun;

/// <summary>
/// Setup script to ensure MultiplayerDoor01Trigger has proper Photon networking components
/// This script should be attached to the same GameObject as MultiplayerDoor01Trigger
/// </summary>
public class MultiplayerDoorSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    public bool autoSetupPhotonView = true;
    
    private MultiplayerDoor01Trigger doorTrigger;
    private PhotonView photonView;

    void Awake()
    {
        // Get or add the door trigger component
        doorTrigger = GetComponent<MultiplayerDoor01Trigger>();
        if (doorTrigger == null)
        {
            Debug.LogError("[MultiplayerDoorSetup] MultiplayerDoor01Trigger component not found on GameObject: " + gameObject.name);
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
            Debug.Log("[MultiplayerDoorSetup] Added PhotonView component to: " + gameObject.name);
        }

        // Configure PhotonView settings
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
            Debug.Log("[MultiplayerDoorSetup] MultiplayerDoor01Trigger setup complete for: " + gameObject.name);
        }
        else
        {
            Debug.LogError("[MultiplayerDoorSetup] Setup incomplete for: " + gameObject.name);
        }
    }
}
