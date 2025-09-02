using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DecreaseSizeButton : MonoBehaviour
{
    public float decreaseCooldown = 0.5f; // seconds
    private bool canDecrease = true;
    public Player player;
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
                if (canDecrease && player != null)
                {
                    player.ChangeSize(0.8f);
                    canDecrease = false;
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
