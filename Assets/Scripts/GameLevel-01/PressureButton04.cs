using UnityEngine;

using UnityEngine;

public class PressureButton04 : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    public GameObject doorTrigger; // Assign MultiplayerDoor01Trigger GameObject
    public Sprite pressedSprite;
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
        if (doorScript != null && buttonIndex >= 0)
            doorScript.OnButtonCollide(buttonIndex);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (doorScript != null && buttonIndex >= 0)
                doorScript.OnButtonReleased(buttonIndex);
        }
    }
}
