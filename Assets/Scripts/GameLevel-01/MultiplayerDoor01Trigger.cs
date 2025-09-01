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

        if (playerCount == 2)
        {
            // Sequence: 01 -> 04
            if (buttonPressed[0] && !buttonPressed[1])
            {
                // Wait for 04
                Debug.Log("[MultiplayerDoor01Trigger] Button 01 pressed, waiting for 04...");
            }
            else if (buttonPressed[0] && buttonPressed[1])
            {
                // Correct sequence: 01 then 04
                Debug.Log("[MultiplayerDoor01Trigger] Correct sequence! Deactivating door.");
                DeactivateDoor();
            }
            else if (buttonPressed[1] && !buttonPressed[0])
            {
                // 04 pressed before 01: restart scene
                Debug.Log("[MultiplayerDoor01Trigger] Wrong sequence! 04 pressed before 01. Restarting scene.");
                RestartScene();
            }
        }
        else if (playerCount == 3)
        {
            // Sequence: 01 -> 04 -> 05
            if (buttonPressed[0] && !buttonPressed[1] && !buttonPressed[2])
            {
                // Wait for 04
                Debug.Log("[MultiplayerDoor01Trigger] Button 01 pressed, waiting for 04...");
            }
            else if (buttonPressed[0] && buttonPressed[1] && !buttonPressed[2])
            {
                // Wait for 05
                Debug.Log("[MultiplayerDoor01Trigger] Buttons 01 and 04 pressed, waiting for 05...");
            }
            else if (buttonPressed[0] && buttonPressed[1] && buttonPressed[2])
            {
                // Correct sequence
                Debug.Log("[MultiplayerDoor01Trigger] Correct sequence! Deactivating door.");
                DeactivateDoor();
            }
            else if ((buttonPressed[1] && !buttonPressed[0]) || (buttonPressed[2] && (!buttonPressed[0] || !buttonPressed[1])))
            {
                // Wrong order
                Debug.Log("[MultiplayerDoor01Trigger] Wrong sequence! Restarting scene.");
                RestartScene();
            }
        }
        else if (playerCount >= 4)
        {
            // All 3 buttons must be pressed simultaneously (order doesn't matter)
            if (buttonColliding[0] && buttonColliding[1] && buttonColliding[2])
            {
                DeactivateDoor();
            }
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
        if (playerCount >= 4 && doorDeactivated)
        {
            // Start timer to reactivate door if no one is on any button
            if (!buttonColliding[0] && !buttonColliding[1] && !buttonColliding[2])
            {
                if (reactivateCoroutine != null) StopCoroutine(reactivateCoroutine);
                reactivateCoroutine = StartCoroutine(ReactivateDoorAfterDelay());
            }
        }
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
        if (playerCount >= 4 && !doorDeactivated)
        {
            if (buttonColliding[0] && buttonColliding[1] && buttonColliding[2])
            {
                DeactivateDoor();
            }
        }
        // Cancel reactivation if someone steps back on
        if (reactivateCoroutine != null)
        {
            StopCoroutine(reactivateCoroutine);
            reactivateCoroutine = null;
        }
    }

    private void DeactivateDoor()
    {
        if (doorToDeactivate != null)
            doorToDeactivate.SetActive(false);
        doorDeactivated = true;
    }

    private IEnumerator ReactivateDoorAfterDelay()
    {
        yield return new WaitForSeconds(doorReactivateDelay);
        if (doorToDeactivate != null)
            doorToDeactivate.SetActive(true);
        doorDeactivated = false;
        // Reset button states for next round
        buttonPressed[0] = buttonPressed[1] = buttonPressed[2] = false;
    }

    private void RestartScene()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("[MultiplayerDoor01Trigger] Initiating scene restart for all clients...");
            
            // Use RPC to ensure all clients restart the scene
            photonView.RPC("RPC_RestartScene", RpcTarget.All);
            
            // Also try the master client approach as backup
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(MasterClientRestartBackup());
            }
        }
        else
        {
            // Offline mode
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator MasterClientRestartBackup()
    {
        // Wait a bit longer than the RPC approach
        yield return new WaitForSeconds(0.5f);
        
        // If we're still the master client and in the room, force restart
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            Debug.Log("[MultiplayerDoor01Trigger] Master client backup restart triggered");
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            PhotonNetwork.LoadLevel(currentSceneName);
        }
    }

    // Alternative method to force scene sync if needed
    [PunRPC]
    private void RPC_ForceSceneSync()
    {
        Debug.Log("[MultiplayerDoor01Trigger] Force scene sync triggered");
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // Force reload the current scene
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LoadLevel(currentSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
        }
    }

    [PunRPC]
    private void RPC_RestartScene()
    {
        Debug.Log("[MultiplayerDoor01Trigger] Restarting scene for all clients...");
        
        // Add a small delay to ensure the RPC is processed by all clients
        StartCoroutine(RestartSceneWithDelay());
    }

    private IEnumerator RestartSceneWithDelay()
    {
        // Wait a frame to ensure all RPCs are processed
        yield return null;
        
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"[MultiplayerDoor01Trigger] Loading scene: {currentSceneName}");
        
        // Ensure we're still in a room before loading
        if (PhotonNetwork.InRoom)
        {
            // Use PhotonNetwork.LoadLevel for all clients to ensure synchronization
            PhotonNetwork.LoadLevel(currentSceneName);
        }
        else
        {
            // Fallback to regular scene loading if not in room
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
        }
    }

    // Helper for PressureButton scripts to register their index
    public int GetButtonIndex(GameObject buttonObj)
    {
        if (buttonObj == pressureButton01) return 0;
        if (buttonObj == pressureButton04) return 1;
        if (buttonObj == pressureButton05) return 2;
        return -1;
    }
}
