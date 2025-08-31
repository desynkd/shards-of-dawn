using UnityEngine;
using Photon.Pun;

public class PressureButton01 : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPressed && other.CompareTag(playerTag))
        {
            int playerCount = 0;
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            // If not in a Photon room, treat as developer/offline mode (playerCount = 0)

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
                // Multiplayer logic (more than 1 player)
                // TODO: Implement multiplayer-specific logic here
                Debug.Log("[PressureButton01] Multiplayer mode: implement custom logic here.");
            }
        }
    }
}
