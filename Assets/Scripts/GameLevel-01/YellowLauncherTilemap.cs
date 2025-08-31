using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class YellowLauncherTilemap : MonoBehaviour
{
    [Header("Launcher Settings")]
    public float launchForce = 20f; // Force applied to launch players
    public Vector2 launchDirection = Vector2.right; // Direction to launch (right by default)
    public string playerTag = "Player"; // Tag to identify player objects
    
    [Header("Tilemap Settings")]
    public Tilemap launcherTilemap; // Assign the YellowLauncher Tilemap in Inspector
    public TileBase activeTile; // Assign the active launcher tile
    public TileBase inactiveTile; // Assign the inactive launcher tile
    
    private bool isActive = false;
    private Collider2D launcherCollider;

    void Start()
    {
        launcherCollider = GetComponent<Collider2D>();
        
        // Initialize launcher state based on player count
        UpdateLauncherState();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;
        
        if (other.CompareTag(playerTag))
        {
            // Launch the player
            LaunchPlayer(other.gameObject);
        }
    }

    void LaunchPlayer(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Apply launch force in the specified direction
            Vector2 launchVelocity = launchDirection.normalized * launchForce;
            playerRb.linearVelocity = launchVelocity;
            
            Debug.Log($"[YellowLauncherTilemap] Launched player with force: {launchVelocity}");
        }
    }

    // Public method to set launcher active/inactive state
    public void SetLauncherActive(bool active)
    {
        isActive = active;
        
        // Update visual appearance by changing tiles
        if (launcherTilemap != null)
        {
            // Get all tiles in the tilemap and change them
            BoundsInt bounds = launcherTilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase currentTile = launcherTilemap.GetTile(tilePosition);
                    
                    // If there's a tile at this position, replace it with the appropriate state tile
                    if (currentTile != null)
                    {
                        if (active && activeTile != null)
                        {
                            launcherTilemap.SetTile(tilePosition, activeTile);
                        }
                        else if (!active && inactiveTile != null)
                        {
                            launcherTilemap.SetTile(tilePosition, inactiveTile);
                        }
                    }
                }
            }
        }
        
        // Enable/disable collider
        if (launcherCollider != null)
        {
            launcherCollider.enabled = active;
        }
        
        Debug.Log($"[YellowLauncherTilemap] Launcher set to {(active ? "active" : "inactive")}");
    }

    // Public method to get current active state
    public bool IsLauncherActive()
    {
        return isActive;
    }

    void UpdateLauncherState()
    {
        int playerCount = GetPlayerCount();
        
        // Set initial state based on player count
        if (playerCount < 2)
        {
            // For testing purposes, activate launcher when player count is 0 (offline mode)
            if (playerCount == 0)
            {
                SetLauncherActive(true);
                Debug.Log("[YellowLauncherTilemap] Launcher activated for testing (offline mode)");
            }
            else
            {
                // Launcher inactive for 1 player (online single player)
                SetLauncherActive(false);
            }
        }
        else
        {
            // Launcher active for 2+ players
            SetLauncherActive(true);
        }
    }

    int GetPlayerCount()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
        {
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }
        return 0; // Not in a room, treat as developer/offline mode
    }

    // Public method to reset launcher state (useful for testing or level reset)
    public void ResetLauncher()
    {
        UpdateLauncherState();
    }

    // Optional: Visual debugging in the scene view
    void OnDrawGizmosSelected()
    {
        // Draw launch direction
        Gizmos.color = Color.yellow;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(launchDirection.normalized * 2f);
        Gizmos.DrawLine(startPos, endPos);
        
        // Draw arrow head
        Vector3 arrowHead = endPos - (Vector3)(launchDirection.normalized * 0.5f);
        Vector3 arrowLeft = arrowHead + Vector3.Cross(launchDirection.normalized, Vector3.forward) * 0.3f;
        Vector3 arrowRight = arrowHead - Vector3.Cross(launchDirection.normalized, Vector3.forward) * 0.3f;
        Gizmos.DrawLine(endPos, arrowLeft);
        Gizmos.DrawLine(endPos, arrowRight);
    }
}
