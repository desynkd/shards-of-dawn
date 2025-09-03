using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class IncreaseSizeButton : MonoBehaviourPun
{
    public float increaseFactor = 1.2f;
    public TileBase pressedTile;
    public TileBase defaultTile; // Assign in inspector
    private Tilemap tilemap;
    private bool isPressed = false;
    private bool canIncrease = true;
    private Vector3Int lastPressedCell;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            tilemap = GetComponentInParent<Tilemap>();
        }
        if (tilemap == null)
        {
            Debug.LogWarning("IncreaseSizeButton: no Tilemap found on object or parent.", this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPressed) return;

        GameObject collidingObject = collision.gameObject;
        if (!collidingObject.CompareTag("Player")) return;

        Player player = collidingObject.GetComponent<Player>();
        if (player == null) return;

        // Only local player can trigger size change in multiplayer
        if (player.pv != null && !player.pv.IsMine) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f) // Player is above button
            {
                // Update the visual state of the button locally
                if (tilemap != null && pressedTile != null)
                {
                    Vector3Int cellPos = tilemap.WorldToCell(contact.point);
                    if (!tilemap.HasTile(cellPos))
                    {
                        Vector3Int alt = tilemap.WorldToCell(transform.position);
                        if (tilemap.HasTile(alt)) cellPos = alt;
                    }
                    tilemap.SetTile(cellPos, pressedTile);
                    lastPressedCell = cellPos;
                }

                // Only change size if we haven't done so already
                if (canIncrease)
                {
                    // Sync the size change across the network
                    if (PhotonNetwork.IsConnected && player.pv != null)
                    {
                        // Use the player's PhotonView to sync the size change
                        player.pv.RPC("RPC_ChangeSize", RpcTarget.All, increaseFactor);
                    }
                    else
                    {
                        // Fallback for single player
                        player.ChangeSize(increaseFactor);
                    }
                    canIncrease = false;

                    // Debug log
                    Debug.Log($"[IncreaseSizeButton] Player {(player.pv?.OwnerActorNr.ToString() ?? "local")} size increased by factor {increaseFactor}");
                }

                isPressed = true;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Player player = collision.gameObject.GetComponent<Player>();
        if (player == null) return;

        // Only local player can reset button in multiplayer
        if (player.pv != null && !player.pv.IsMine) return;

        if (tilemap != null && defaultTile != null)
        {
            tilemap.SetTile(lastPressedCell, defaultTile);
        }
        isPressed = false;
        canIncrease = true;
    }
}
