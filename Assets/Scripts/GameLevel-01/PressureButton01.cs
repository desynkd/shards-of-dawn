using UnityEngine;
using Photon.Pun;

public class PressureButton01 : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    public GameObject targetToDeactivate; // Assign in Inspector (for single player mode)
    public GameObject doorTrigger; // Assign MultiplayerDoor01Trigger GameObject
    public Sprite pressedSprite; // Assign the new sprite in Inspector
    public string playerTag = "Player"; // Tag to identify player objects

    private SpriteRenderer spriteRenderer;
    private bool isPressed = false;
    private MultiplayerDoor01Trigger doorScript;
    private int buttonIndex = -1;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Get reference to the door trigger script
        if (doorTrigger != null)
        {
            doorScript = doorTrigger.GetComponent<MultiplayerDoor01Trigger>();
            if (doorScript != null)
                buttonIndex = doorScript.GetButtonIndex(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPressed && other.CompareTag(playerTag))
        {
            int playerCount = 0;
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            else
                playerCount = 1; // Offline/dev mode

            if (playerCount <= 1)
            {
                // Single player or developer mode logic
                isPressed = true;
                if (targetToDeactivate != null)
                    targetToDeactivate.SetActive(false);
                if (spriteRenderer != null && pressedSprite != null)
                    spriteRenderer.sprite = pressedSprite;
            }
            else
            {
                // Multiplayer logic (2 or more players)
                isPressed = true;
                if (spriteRenderer != null && pressedSprite != null)
                    spriteRenderer.sprite = pressedSprite;
                
                // Notify the door trigger system
                if (doorScript != null && buttonIndex >= 0)
                {
                    doorScript.OnButtonPressed(buttonIndex);
                }
            }
        }
        
        // Collision tracking removed - no longer needed
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Button stays pressed permanently - no reset logic
        // Once pressed, the button remains in pressed state
    }
}
