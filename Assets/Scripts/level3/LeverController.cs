using UnityEngine;
using UnityEngine.Tilemaps;

public class LeverController : MonoBehaviour
{
    private Tilemap leverTilemap;
    public TileBase triggeredTile; // Assign in inspector
    public LiftController lift; // Assign in inspector
    private bool isTriggered = false;
    private Vector3Int lastCellPos;
    private TileBase originalTile;
    private bool hasOriginalTile = false;

    private void Start()
    {
        leverTilemap = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided with lever.");
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Debug.Log($"Contact at {contact.point}, normal {contact.normal}");
                // Optionally, check contact.normal.y < -0.5f for top collision
                if (leverTilemap != null && triggeredTile != null)
                {
                    Vector3 hitWorldPos = contact.point;
                    Vector3Int cellPos = leverTilemap.WorldToCell(hitWorldPos);
                    Debug.Log($"Lever contact cell {cellPos}.");

                    // Store original tile once for this cell so we can restore it later
                    if (!hasOriginalTile)
                    {
                        originalTile = leverTilemap.GetTile(cellPos);
                        lastCellPos = cellPos;
                        hasOriginalTile = true;
                    }

                    // If we hit a different cell than we originally stored, update stored values
                    if (hasOriginalTile && cellPos != lastCellPos)
                    {
                        originalTile = leverTilemap.GetTile(cellPos);
                        lastCellPos = cellPos;
                    }

                    // Toggle the tile: if currently triggered, restore original; otherwise set triggered tile
                    TileBase current = leverTilemap.GetTile(cellPos);
                    if (current == triggeredTile || isTriggered)
                    {
                        leverTilemap.SetTile(cellPos, originalTile);
                        Debug.Log($"Restored original tile at {cellPos}.");
                    }
                    else
                    {
                        leverTilemap.SetTile(cellPos, triggeredTile);
                        Debug.Log($"Set triggered tile at {cellPos}.");
                    }

                    // Toggle state and trigger the lift movement
                    isTriggered = !isTriggered;
                    if (lift != null)
                    {
                        Debug.Log("Triggering lift movement.");
                        lift.MoveLift();
                    }

                    break; // only handle first relevant contact
                }
            }
        }
    }
}
