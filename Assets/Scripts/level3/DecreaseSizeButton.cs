using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class DecreaseSizeButton : MonoBehaviourPun
{
    public float decreaseFactor = 0.8f;
    public float decreaseCooldown = 0.5f; // seconds
    private bool canDecrease = true;
    public TileBase pressedTile;
    public TileBase defaultTile; // Assign in inspector
    private Tilemap tilemap;
    private bool isPressed = false;
    private Vector3Int lastPressedCell;
    private HashSet<GameObject> currentPlayers = new HashSet<GameObject>();
    public float releaseDelay = 0.05f; // small delay before releasing to avoid bounce
    private Coroutine pendingRelease;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            tilemap = GetComponentInParent<Tilemap>();
        }
        if (tilemap == null)
        {
            Debug.LogWarning("DecreaseSizeButton: no Tilemap found on object or parent.", this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Get the Player component
        Player player = collision.gameObject.GetComponent<Player>();
        if (player == null) return;

        // Only local player can trigger size change in multiplayer
        if (player.pv != null && !player.pv.IsMine) return;

        // Cancel pending release if player re-enters quickly
        if (pendingRelease != null)
        {
            StopCoroutine(pendingRelease);
            pendingRelease = null;
        }

        // Use GameObject tracking to avoid multiple collider jitter
        if (!currentPlayers.Contains(collision.gameObject))
            currentPlayers.Add(collision.gameObject);

        // Only handle press when first player arrives
        if (currentPlayers.Count != 1 || isPressed) return;

        // Require at least one contact with downward normal (player above)
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                if (tilemap != null && pressedTile != null)
                {
                    Vector3Int cellPos = tilemap.WorldToCell(contact.point);
                    if (!tilemap.HasTile(cellPos))
                    {
                        Vector3Int alt = tilemap.WorldToCell(transform.position);
                        if (tilemap.HasTile(alt)) cellPos = alt;
                    }
                    tilemap.SetTile(cellPos, pressedTile);
                    tilemap.RefreshTile(cellPos);
                    lastPressedCell = cellPos;
                }

                if (canDecrease)
                {
                    // Sync the size change across the network
                    if (PhotonNetwork.IsConnected && player.pv != null)
                    {
                        // Use the player's PhotonView to sync the size change
                        player.pv.RPC("RPC_ChangeSize", RpcTarget.All, decreaseFactor);
                    }
                    else
                    {
                        // Fallback for single player
                        player.ChangeSize(decreaseFactor);
                    }
                    canDecrease = false;

                    // Debug log
                    Debug.Log($"[DecreaseSizeButton] Player {(player.pv?.OwnerActorNr.ToString() ?? "local")} size decreased by factor {decreaseFactor}");
                }

                isPressed = true;
                break;
            }
        }
    }

    private System.Collections.IEnumerator ResetDecreaseCooldown()
    {
        yield return new WaitForSeconds(decreaseCooldown);
        canDecrease = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Player player = collision.gameObject.GetComponent<Player>();
        if (player == null) return;

        // Only local player can reset button in multiplayer
        if (player.pv != null && !player.pv.IsMine) return;

        if (currentPlayers.Contains(collision.gameObject))
            currentPlayers.Remove(collision.gameObject);

        // Start a short delayed release to avoid immediate bounce
        if (currentPlayers.Count == 0)
        {
            if (pendingRelease != null) StopCoroutine(pendingRelease);
            pendingRelease = StartCoroutine(DelayedRelease());
        }
    }

    private System.Collections.IEnumerator DelayedRelease()
    {
        yield return new WaitForSeconds(releaseDelay);
        // if no players re-entered during the delay, release
        if (currentPlayers.Count == 0)
        {
            if (tilemap != null)
            {
                tilemap.SetTile(lastPressedCell, defaultTile);
                tilemap.RefreshTile(lastPressedCell);
            }
            isPressed = false;
            StartCoroutine(ResetDecreaseCooldown());
        }
        pendingRelease = null;
    }
}
