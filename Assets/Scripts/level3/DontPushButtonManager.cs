
using UnityEngine;
using UnityEngine.Tilemaps;

public class DontPushButtonManager : MonoBehaviour
{
    public TileBase pressedTile; // Assign in Inspector
    private bool respawnTriggered = false;
    private Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (respawnTriggered) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    if (tilemap != null && pressedTile != null)
                    {
                        Vector3 hitWorldPos = contact.point;
                        Vector3Int cellPos = tilemap.WorldToCell(hitWorldPos);
                        tilemap.SetTile(cellPos, pressedTile);
                    }
                    respawnTriggered = true;
                    LevelRespawnManager.RespawnCurrentLevel();
                    break;
                }
            }
        }
    }
}
