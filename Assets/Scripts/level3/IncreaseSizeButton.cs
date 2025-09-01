using UnityEngine;
using UnityEngine.Tilemaps;

public class IncreaseSizeButton : MonoBehaviour
{
    public Player player;
    public Tilemap tilemap;
    public TileBase pressedTile;
    public Vector3Int buttonTilePosition;

    void Awake() { }

    public void OnButtonPress()
    {
        if (player != null)
        {
            player.ChangeSize(1.2f); // Increase size by 20%
        }
        if (tilemap != null && pressedTile != null)
            tilemap.SetTile(buttonTilePosition, pressedTile);
    }

    public void OnButtonRelease()
    {
        if (tilemap != null)
            tilemap.SetTile(buttonTilePosition, null); // Revert to default design
    }

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if player is above the button
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null && rb.velocity.y < 0)
            {
                OnButtonPress();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnButtonRelease();
        }
    }
}
