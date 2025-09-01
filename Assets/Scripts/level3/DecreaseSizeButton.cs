using UnityEngine;
using UnityEngine.Tilemaps;

public class DecreaseSizeButton : MonoBehaviour
{
    public float decreaseCooldown = 0.5f; // seconds
    private bool canDecrease = true;
    public Player player;
    public Tilemap tilemap;
    public TileBase pressedTile;
    public Vector3Int buttonTilePosition;
    void Awake() { }

    public void OnButtonPress()
    {
        if (canDecrease && player != null)
        {
            player.ChangeSize(0.8f); // Decrease size by 20%
            canDecrease = false;
            StartCoroutine(ResetDecreaseCooldown());
        }
        if (tilemap != null && pressedTile != null)
            tilemap.SetTile(buttonTilePosition, pressedTile);
    }

    private System.Collections.IEnumerator ResetDecreaseCooldown()
    {
        yield return new WaitForSeconds(decreaseCooldown);
        canDecrease = true;
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
