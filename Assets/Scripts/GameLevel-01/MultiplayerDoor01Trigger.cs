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
    private bool[] buttonColliding = new bool[3]; // For 4 players, track if any player is on each button
    private bool doorDeactivated = false;
    private Coroutine reactivateCoroutine;

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

        int requiredButtons = Mathf.Min(playerCount, 3);
        int pressedCount = 0;
        for (int i = 0; i < 3; i++) if (buttonPressed[i]) pressedCount++;

        if (pressedCount >= requiredButtons)
        {
            DeactivateDoor();
        }
    }

    public void OnButtonReleased(int buttonIndex)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_OnButtonReleased", RpcTarget.All, buttonIndex);
        }
        else
        {
            HandleButtonReleased(buttonIndex);
        }
    }

    [PunRPC]
    private void RPC_OnButtonReleased(int buttonIndex)
    {
        HandleButtonReleased(buttonIndex);
    }

    private void HandleButtonReleased(int buttonIndex)
    {
        buttonColliding[buttonIndex] = false;
    }

    public void OnButtonCollide(int buttonIndex)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_OnButtonCollide", RpcTarget.All, buttonIndex);
        }
        else
        {
            HandleButtonCollide(buttonIndex);
        }
    }

    [PunRPC]
    private void RPC_OnButtonCollide(int buttonIndex)
    {
        HandleButtonCollide(buttonIndex);
    }

    private void HandleButtonCollide(int buttonIndex)
    {
        buttonColliding[buttonIndex] = true;
    }

    private void DeactivateDoor()
    {
        if (doorToDeactivate != null)
            doorToDeactivate.SetActive(false);
        doorDeactivated = true;
    }

    private IEnumerator ReactivateDoorAfterDelay()
    {
        yield return null;
    }

    private void RestartScene(){}

    private IEnumerator MasterClientRestartBackup(){yield return null;}

    // Alternative method to force scene sync if needed
    [PunRPC]
    private void RPC_ForceSceneSync(){}

    [PunRPC]
    private void RPC_RestartScene(){}

    private IEnumerator RestartSceneWithDelay(){yield return null;}

    // Helper for PressureButton scripts to register their index
    public int GetButtonIndex(GameObject buttonObj)
    {
        if (buttonObj == pressureButton01) return 0;
        if (buttonObj == pressureButton04) return 1;
        if (buttonObj == pressureButton05) return 2;
        return -1;
    }
}
