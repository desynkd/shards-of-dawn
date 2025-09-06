
using UnityEngine;
using UnityEngine.Tilemaps;

public class DontPushButtonManager : MonoBehaviour
{
    public TileBase pressedTile; // Assign in Inspector
    private bool respawnTriggered = false;
    private Tilemap tilemap;

    // Reference to a box collider for top-only collision
    private BoxCollider2D topCollider;

    public bool debugMode = true; // Enable debug by default

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        // Setup the collider for top-only collision
        SetupTopCollider();
    }

    void SetupTopCollider()
    {
        // Get or create a box collider
        topCollider = GetComponent<BoxCollider2D>();
        if (topCollider == null)
        {
            topCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("Added BoxCollider2D to button");
        }

        // Make the collider smaller and positioned at the top of the button
        // Adjust these values as needed based on your button size
        topCollider.size = new Vector2(0.8f, 0.2f);
        topCollider.offset = new Vector2(0, 0.4f); // Offset to the top of the button

        if (debugMode)
            Debug.Log("Top collider set up at position: " + transform.position);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Early return if already triggered or not player
        if (respawnTriggered || !collision.gameObject.CompareTag("Player"))
            return;

        if (debugMode)
            Debug.Log("Collision with Player detected at: " + collision.contacts[0].point);

        // Check for downward velocity of the player
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null && playerRb.linearVelocity.y < -0.1f)
        {
            if (debugMode)
                Debug.Log("Player is moving downward with velocity: " + playerRb.linearVelocity.y);

            // Get collision point and cell position
            Vector3 hitPos = collision.contacts[0].point;
            Vector3Int cellPos = tilemap.WorldToCell(hitPos);

            // Press the button!
            if (tilemap != null && pressedTile != null)
            {
                tilemap.SetTile(cellPos, pressedTile);

                if (debugMode)
                    Debug.Log("Button pressed at cell: " + cellPos);
            }

            respawnTriggered = true;

            // Add a small delay before respawning to allow the player to see the pressed button
            Invoke("RespawnLevel", 0.2f);
        }
    }

    // Handle trigger-based detection as well
    void OnTriggerEnter2D(Collider2D other)
    {
        // Early return if already triggered or not player
        if (respawnTriggered || !other.CompareTag("Player"))
            return;

        if (debugMode)
            Debug.Log("Trigger with Player at: " + other.transform.position);

        // Check if the player is moving downward
        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb != null && playerRb.linearVelocity.y < -0.1f)
        {
            if (debugMode)
                Debug.Log("Player triggering from above with velocity: " + playerRb.linearVelocity.y);

            // Get player position and convert to cell
            Vector3 playerPos = other.transform.position;
            Vector3Int cellPos = tilemap.WorldToCell(playerPos);

            // Press the button!
            if (tilemap != null && pressedTile != null)
            {
                tilemap.SetTile(cellPos, pressedTile);

                if (debugMode)
                    Debug.Log("Button triggered at cell: " + cellPos);
            }

            respawnTriggered = true;

            // Add a small delay before respawning
            Invoke("RespawnLevel", 0.2f);
        }
    }

    void RespawnLevel()
    {
        LevelRespawnManager.RespawnCurrentLevel();
    }
}

