using UnityEngine;

public class PressureButton02 : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    public GameObject targetToDeactivate; // Assign in Inspector
    public Sprite pressedSprite; // Assign the new sprite in Inspector
    public string playerTag = "Player"; // Tag to identify player objects

    private SpriteRenderer spriteRenderer;
    private bool isPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPressed && other.CompareTag(playerTag))
        {
            isPressed = true;
            if (targetToDeactivate != null)
                targetToDeactivate.SetActive(false);
            if (spriteRenderer != null && pressedSprite != null)
                spriteRenderer.sprite = pressedSprite;
        }
    }
}
