using UnityEngine;
using UnityEngine.Tilemaps;

public class IncreaseSizeButton : MonoBehaviour
{
    public Player player;
    public TileBase pressedTile;
    public TileBase defaultTile; // Assign in inspector
    private Tilemap tilemap;
    private bool isPressed = false;
    private bool canIncrease = true;
    private Vector3Int lastPressedCell;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPressed) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (tilemap != null && pressedTile != null)
                {
                    Vector3Int cellPos = tilemap.WorldToCell(contact.point);
                    tilemap.SetTile(cellPos, pressedTile);
                    lastPressedCell = cellPos;
                }
                if (canIncrease && player != null)
                {
                    player.ChangeSize(1.2f);
                    canIncrease = false;
                }
                isPressed = true;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (tilemap != null)
            {
                tilemap.SetTile(lastPressedCell, defaultTile);
            }
            isPressed = false;
            canIncrease = true;
        }
    }

}
