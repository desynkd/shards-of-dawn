using UnityEngine;

public class PressureButton05 : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    public GameObject doorTrigger; // Assign MultiplayerDoor01Trigger GameObject in Inspector
    public Sprite pressedSprite; // Assign the pressed sprite in Inspector
    public string playerTag = "Player";

    private SpriteRenderer spriteRenderer;
    private bool isPressed = false;
    private MultiplayerDoor01Trigger doorScript;
    private int buttonIndex = -1;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            isPressed = true;
            if (spriteRenderer != null && pressedSprite != null)
                spriteRenderer.sprite = pressedSprite;
            if (doorScript != null && buttonIndex >= 0)
                doorScript.OnButtonPressed(buttonIndex);
        }
        // Collision tracking removed - no longer needed
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Button stays pressed permanently - no reset logic
        // Once pressed, the button remains in pressed state
    }
}
