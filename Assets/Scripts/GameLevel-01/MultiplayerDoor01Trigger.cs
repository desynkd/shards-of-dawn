using UnityEngine;

using UnityEngine;
using Photon.Pun;
using System.Collections;

public class MultiplayerDoor01Trigger : MonoBehaviour
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

        // Set initial button/door states
        if (doorToDeactivate != null)
            doorToDeactivate.SetActive(true);
        if (pressureButton04 != null)
            pressureButton04.SetActive(false);
        if (pressureButton05 != null)
            pressureButton05.SetActive(false);

        // Activate buttons based on player count
        if (playerCount == 2)
        {
            if (pressureButton04 != null) pressureButton04.SetActive(true);
        }
        else if (playerCount == 3)
        {
            if (pressureButton04 != null) pressureButton04.SetActive(true);
            if (pressureButton05 != null) pressureButton05.SetActive(true);
        }
        else if (playerCount == 4)
        {
            if (pressureButton04 != null) pressureButton04.SetActive(true);
            if (pressureButton05 != null) pressureButton05.SetActive(true);
        }
    }

    // Called by PressureButton scripts
    public void OnButtonPressed(int buttonIndex)
    {
        if (doorDeactivated) return;
        buttonPressed[buttonIndex] = true;

        if (playerCount == 2)
        {
            // Sequence: 01 -> 04
            if (buttonPressed[0] && !buttonPressed[1])
            {
                // Wait for 04
            }
            else if (buttonPressed[0] && buttonPressed[1])
            {
                // Correct sequence: 01 then 04
                DeactivateDoor();
            }
            else if (buttonPressed[1] && !buttonPressed[0])
            {
                // 04 pressed before 01: restart scene
                RestartScene();
            }
        }
        else if (playerCount == 3)
        {
            // Sequence: 01 -> 04 -> 05
            if (buttonPressed[0] && !buttonPressed[1] && !buttonPressed[2])
            {
                // Wait for 04
            }
            else if (buttonPressed[0] && buttonPressed[1] && !buttonPressed[2])
            {
                // Wait for 05
            }
            else if (buttonPressed[0] && buttonPressed[1] && buttonPressed[2])
            {
                // Correct sequence
                DeactivateDoor();
            }
            else if ((buttonPressed[1] && !buttonPressed[0]) || (buttonPressed[2] && (!buttonPressed[0] || !buttonPressed[1])))
            {
                // Wrong order
                RestartScene();
            }
        }
        else if (playerCount == 4)
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
        buttonColliding[buttonIndex] = false;
        if (playerCount == 4 && doorDeactivated)
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
        buttonColliding[buttonIndex] = true;
        if (playerCount == 4 && !doorDeactivated)
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
        PhotonNetwork.LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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
