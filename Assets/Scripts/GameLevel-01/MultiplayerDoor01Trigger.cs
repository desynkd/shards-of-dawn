using UnityEngine;
using Photon.Pun;
using System.Collections;

public class MultiplayerDoor01Trigger : MonoBehaviourPun
{
    [Header("Door and Button References")]
    public GameObject doorToDeactivate; // Assign Door 01 here
    public GameObject pressureButton01; // Assign in Inspector
    public GameObject pressureButton04; // Assign in Inspector
    public GameObject pressureButton05; // Assign in Inspector
    public float doorReactivateDelay = 10f; // Editable in Inspector for 4 players

    private int playerCount = 0;
    private bool[] buttonPressed = new bool[3]; // 0:01, 1:04, 2:05
    private bool doorDeactivated = false;

    void Start()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        else
            playerCount = 1; // Offline/dev mode

        Debug.Log($"[MultiplayerDoor01Trigger] Initialized with player count: {playerCount}");

        // Set initial button/door states
        if (doorToDeactivate != null)
            doorToDeactivate.SetActive(true);
        if (pressureButton04 != null)
            pressureButton04.SetActive(false);
        if (pressureButton05 != null)
            pressureButton05.SetActive(false);

        // Activate buttons based on player count
        if (playerCount >= 2)
        {
            if (pressureButton04 != null) 
            {
                pressureButton04.SetActive(true);
                Debug.Log("[MultiplayerDoor01Trigger] Activated Button 04 for 2+ players");
            }
        }
        if (playerCount >= 3)
        {
            if (pressureButton05 != null) 
            {
                pressureButton05.SetActive(true);
                Debug.Log("[MultiplayerDoor01Trigger] Activated Button 05 for 3+ players");
            }
        }

        Debug.Log($"[MultiplayerDoor01Trigger] New Logic - 1 player: Button 01 only, 2 players: Buttons 01+04, 3+ players: All three buttons");
    }

    // Called by PressureButton scripts - now uses RPC for multiplayer synchronization
    public void OnButtonPressed(int buttonIndex)
    {
        if (doorDeactivated) return;
        
        // Use RPC to synchronize button press across all clients
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_OnButtonPressed", RpcTarget.All, buttonIndex);
        }
        else
        {
            // Offline mode - call directly
            HandleButtonPressed(buttonIndex);
        }
    }

    [PunRPC]
    private void RPC_OnButtonPressed(int buttonIndex)
    {
        HandleButtonPressed(buttonIndex);
    }

    private void HandleButtonPressed(int buttonIndex)
    {
        if (doorDeactivated) return;
        buttonPressed[buttonIndex] = true;

        Debug.Log($"[MultiplayerDoor01Trigger] Button {buttonIndex} pressed. Player count: {playerCount}. Button states: 01={buttonPressed[0]}, 04={buttonPressed[1]}, 05={buttonPressed[2]}");

        // Check if door should be opened based on new logic
        CheckDoorConditions();
    }

    // Button release logic removed - buttons stay pressed permanently once activated
    public void OnButtonReleased(int buttonIndex)
    {
        // No action needed - buttons don't release in new logic
    }

    public void OnButtonCollide(int buttonIndex)
    {
        // No action needed - collision tracking not required in new logic
    }

    private void CheckDoorConditions()
    {
        if (doorDeactivated) return;

        bool shouldOpenDoor = false;

        if (playerCount <= 1)
        {
            // Single player or offline: only need button 01
            shouldOpenDoor = buttonPressed[0];
            Debug.Log($"[MultiplayerDoor01Trigger] Single player mode: Button 01 pressed = {buttonPressed[0]}");
        }
        else if (playerCount == 2)
        {
            // Two players: need buttons 01 AND 04 (no order requirement)
            shouldOpenDoor = buttonPressed[0] && buttonPressed[1];
            Debug.Log($"[MultiplayerDoor01Trigger] Two player mode: Button 01 = {buttonPressed[0]}, Button 04 = {buttonPressed[1]}");
        }
        else // playerCount >= 3
        {
            // Three or more players: need all three buttons (no order requirement)
            shouldOpenDoor = buttonPressed[0] && buttonPressed[1] && buttonPressed[2];
            Debug.Log($"[MultiplayerDoor01Trigger] Three+ player mode: Button 01 = {buttonPressed[0]}, Button 04 = {buttonPressed[1]}, Button 05 = {buttonPressed[2]}");
        }

        if (shouldOpenDoor)
        {
            DeactivateDoor();
        }
    }

    private void DeactivateDoor()
    {
        if (doorToDeactivate != null)
            doorToDeactivate.SetActive(false);
        doorDeactivated = true;
        Debug.Log("[MultiplayerDoor01Trigger] Door permanently deactivated!");
    }

    // Removed unused restart scene methods - no longer needed with new logic

    // Helper for PressureButton scripts to register their index
    public int GetButtonIndex(GameObject buttonObj)
    {
        if (buttonObj == pressureButton01) return 0;
        if (buttonObj == pressureButton04) return 1;
        if (buttonObj == pressureButton05) return 2;
        return -1;
    }
}
